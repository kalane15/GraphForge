import "@/styles/common.css";


function ConfirmForm({ onConfirm, onClose }) {
    async function handleConfirm() {
        await onConfirm();
        onClose();
    }

    return (
        <div className="modal-overlay">
            <div className="modal">
                <h2>Are you sure?</h2>

                <button type="button" onClick={onClose}>
                    Cancel
                </button>

                <button type="button" onClick={handleConfirm}>
                    Ok
                </button>
            </div>
        </div>
    )
}

export default ConfirmForm;
