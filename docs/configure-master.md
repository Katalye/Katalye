Master configuration:

```yml
state_events: True # Enables progress events for jobs.
presence_events: False # Enables minion presence detection, not currently used.

event_return: katalye
katalye.server: w4 # String, the resolvable Katalye server, this may also contain a port, required.
katalye.secure: False # Boolean, defaults to False.
```