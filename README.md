# MockIOT
A mock implementation of an IOT device service.

The MockIOT project is an ASP.NET Core that has a set of pre-defined device ids and events. It then uses SSE to "ping" an event from the list every 1 second to any registered listerners.
It has the following endpoints:
1. `/api/{deviceId}` - This is the end point used to subscribe to a stream of events for a given device.
2. `/` - This returns an example HTML page tha uses JavaScript to subscribe to an SSE stream.


The Client project is an example F# console app that demonstrates how to consume events from the SSE stream.
