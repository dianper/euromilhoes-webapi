# euromilhoes-webapi
This project aims to expose an API based on the latest EuroMillions results and also expose an endpoint to generate the numbers that never came out in the game.

A background service is responsible for capturing all the results from the beginning when the game was created.

## endpoints
[GET]
- api/euromilhoes/last/{quantity}
   - list with last {quantity} results
- api/euromilhoes/generate
  - i.e: 05-06-07-08-09 01-02
- api/euromilhoes/key/{key}
  - checks if the key exists
- api/euromilhoes/repeated
   - list with repeated keys
