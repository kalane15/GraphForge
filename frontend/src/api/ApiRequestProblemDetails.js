export class ApiRequestProblemDetails extends Error {
    constructor(problemDetails) {
        super(problemDetails.detail ?? problemDetails.title ?? "Request failed");
        this.title = problemDetails.title;
        this.status = problemDetails.status;
        this.detail = problemDetails.detail;
    }
}
