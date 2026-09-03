# Conventions

##All code must satisfy the following conventions:
- All API endpoints in case of error must return a ProblemDetails object.
- All successful API responses that contain a response body must use a defined DTO type rather than an anonymous type.