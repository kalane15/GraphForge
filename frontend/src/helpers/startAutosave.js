export function startAutosave(save) {
    let isSaving = false;

    const intervalId = setInterval(async () => {
        if (isSaving) {
            return;
        }

        isSaving = true;
        try {
            // The caller handles errors and reports them to the user.
            await save();
        } finally {
            isSaving = false;
        }
    }, 10_000);

    return () => clearInterval(intervalId);
}
