export function createSaveQueue(writeContent, initialContent) {
    let pending = Promise.resolve();
    let savedSnapshot = initialContent === undefined ? null : JSON.stringify(initialContent);

    function save(content) {
        const snapshot = JSON.stringify(content);
        const result = pending.then(async () => {
            if (snapshot === savedSnapshot) {
                return false;
            }

            await writeContent(JSON.parse(snapshot));
            savedSnapshot = snapshot;
            return true;
        });

        // A failed write must not prevent subsequent saves or retries.
        pending = result.catch(() => {});
        return result;
    }

    // Deletion must wait for writes already queued for the same resource.
    save.waitForIdle = () => pending;
    return save;
}
