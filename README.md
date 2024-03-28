## Peak Ventures visitors tracking service

Peak Ventures visitors tracking service is aimed to be used by the end users to be able to track visits to their sites.

This service has the following logic:
- GET /track API endpoint returns 1-pixel image in GIF format and collects some information from the user request (referer, user agent, IP address).
- Collected user information is written to a log file for further analysis.

## Architecture overview

This solution consists of the following parts:
- Pixel web service contains user-facing API. End users interact directly with this service. This service collects user information. 
- Storage background worker service. Thi service is responsible for writing user information to the log file. 
- RabbitMQ message queue is used for the asynchronous communication between Pixel service and Storage background worker.
  - Whenever Pixel service API endpoint is called, it collects user information, uses it to construct a message in a specified format, 
  and publishes this message to the message queue.
  - On the other side, Storage background service reads messages from the message queue, and writes received messages to the log file.