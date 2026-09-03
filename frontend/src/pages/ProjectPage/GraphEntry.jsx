import "@/styles/common.css";
import { useNavigate } from "react-router";

function GraphEntry(props) {
    const navigate = useNavigate();

    function formatDate(value) {
        if (!value) {
            return "-";
        }

        return new Date(value).toLocaleString();
    }

    async function loadGraph() {
        navigate(`/graphs/${props.id}`);
    }

    return (
        <div className="list-entry list-entry--graph">
            <div className="list-entry__field">
                <span className="list-entry__label">Name</span>
                <span className="list-entry__value">{props.name}</span>
            </div>

            <div className="list-entry__field">
                <span className="list-entry__label">Id</span>
                <span className="list-entry__value">{props.id}</span>
            </div>

            <div className="list-entry__field">
                <span className="list-entry__label">Project</span>
                <span className="list-entry__value">
                    {props.projectName ?? props.projectId ?? "-"}
                </span>
            </div>

            <div className="list-entry__field">
                <span className="list-entry__label">Created</span>
                <span className="list-entry__value">{formatDate(props.createdAt)}</span>
            </div>

            <div className="list-entry__field">
                <span className="list-entry__label">Updated</span>
                <span className="list-entry__value">{formatDate(props.updatedAt)}</span>
            </div>

            <button className="list-entry__button" onClick={loadGraph}>
                Load
            </button>
        </div>
    );
}

export default GraphEntry;
