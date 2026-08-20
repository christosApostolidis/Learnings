# Middleware Implementation

## Logging Middleware

Logs the HTTP method, request path, and response status code for API requests.

## Error-Handling Middleware

Catches unexpected exceptions, logs the error, and returns a consistent JSON
response with status code 500.

## Authentication Middleware

Checks the Authorization header for a valid Bearer token. Requests without the
correct token receive a 401 Unauthorized response.

## Pipeline Order

The middleware was configured in this order:

1. Error handling
2. Authentication
3. Logging

## Microsoft Copilot

Copilot helped generate the middleware classes and configure the pipeline.
I reviewed the suggested code and tested valid tokens, invalid tokens, logging,
and API responses before accepting the changes.