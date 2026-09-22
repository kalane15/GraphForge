import assert from "node:assert/strict";
import { test } from "node:test";
import { createSaveQueue } from "../src/helpers/createSaveQueue.js";
import { startAutosave } from "../src/helpers/startAutosave.js";
import { ApiRequestProblemDetails } from "../src/api/ApiRequestProblemDetails.js";

test("API errors expose the server detail as the user-visible message", () => {
    const error = new ApiRequestProblemDetails({ title: "Invalid schema", detail: "Field names must be unique", status: 400 });
    assert.equal(error.message, "Field names must be unique");
    assert.equal(error.status, 400);
    assert.equal(new ApiRequestProblemDetails({ title: "Unavailable" }).message, "Unavailable");
});

function deferred() {
    let resolve;
    const promise = new Promise((done) => { resolve = done; });
    return { promise, resolve };
}

function mockIntervals(t) {
    const major = Number(process.versions.node.split(".")[0]);
    t.mock.timers.enable(major >= 22 ? { apis: ["setInterval"] } : ["setInterval"]);
}

test("loaded schemas are not saved again until edited", async () => {
    const original = { id: "schema-a", schemaTypeName: "Original", fields: [] };
    const writes = [];
    const save = createSaveQueue(async (schema) => writes.push(schema), original);

    assert.equal(await save(original), false);
    assert.equal(await save({ ...original, schemaTypeName: "Changed" }), true);
    assert.equal(await save({ ...original, schemaTypeName: "Changed" }), false);
    assert.equal(writes.length, 1);
    assert.equal(writes[0].schemaTypeName, "Changed");
});

test("each schema retains its own last saved snapshot", async () => {
    const first = { id: "a", name: "A" };
    const second = { id: "b", name: "B" };
    const writes = [];
    const write = async (schema) => writes.push(schema);
    const saveFirst = createSaveQueue(write, first);
    const saveSecond = createSaveQueue(write, second);

    await saveFirst({ ...first, name: "A2" });
    await saveSecond({ ...second, name: "B2" });
    await saveFirst({ ...first, name: "A2" });
    assert.deepEqual(writes.map((schema) => schema.id), ["a", "b"]);
});

test("writes are sequential and preserve edits made during an earlier request", async () => {
    const request = deferred();
    const writes = [];
    const save = createSaveQueue(async (schema) => {
        writes.push(schema);
        if (writes.length === 1) await request.promise;
    });
    const first = save({ name: "First" });
    const latest = save({ name: "Latest" });
    await Promise.resolve();
    assert.deepEqual(writes, [{ name: "First" }]);
    request.resolve();
    await Promise.all([first, latest]);
    assert.deepEqual(writes, [{ name: "First" }, { name: "Latest" }]);
});

test("queued snapshots are independent of subsequent object mutations", async () => {
    const writes = [];
    const save = createSaveQueue(async (content) => writes.push(content));
    const draft = { fields: [{ name: "Original" }] };
    const result = save(draft);
    draft.fields[0].name = "Changed";
    await result;
    assert.equal(writes[0].fields[0].name, "Original");
});

test("a failed write remains retryable and does not block the queue", async () => {
    let attempts = 0;
    const save = createSaveQueue(async () => {
        if (++attempts === 1) throw new Error("Offline");
    });
    await assert.rejects(save({ name: "Changed" }), /Offline/);
    assert.equal(await save({ name: "Changed" }), true);
    assert.equal(attempts, 2);
});

test("waitForIdle allows deletion only after an outstanding write completes", async () => {
    const request = deferred();
    const events = [];
    const save = createSaveQueue(async () => {
        await request.promise;
        events.push("saved");
    });
    const saving = save({ name: "Changed" });
    const deletion = save.waitForIdle().then(() => events.push("deleted"));
    await Promise.resolve();
    assert.deepEqual(events, []);
    request.resolve();
    await Promise.all([saving, deletion]);
    assert.deepEqual(events, ["saved", "deleted"]);
});

test("autosave keeps ticking during continuous edits and reads the latest draft", async (t) => {
    mockIntervals(t);
    let draft = 0;
    const writes = [];
    t.after(startAutosave(() => { writes.push(draft); }));
    for (let second = 1; second <= 20; second += 1) {
        draft = second;
        t.mock.timers.tick(1000);
        await Promise.resolve();
    }
    assert.deepEqual(writes, [10, 20]);
});

test("autosave skips busy ticks, resumes after a handled failure, and stops on cleanup", async (t) => {
    mockIntervals(t);
    const request = deferred();
    let calls = 0;
    const stop = startAutosave(() => { calls += 1; return request.promise; });
    t.after(stop);
    t.mock.timers.tick(10_000);
    t.mock.timers.tick(10_000);
    assert.equal(calls, 1);
    request.resolve(false);
    await Promise.resolve();
    t.mock.timers.tick(10_000);
    assert.equal(calls, 2);
    stop();
    await Promise.resolve();
    t.mock.timers.tick(10_000);
    assert.equal(calls, 2);
});
