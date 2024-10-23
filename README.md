# Effective Mobile

## Description

The **Effective Mobile** application is a test project designed for managing orders easily. It is built using **C#**, **ASP.NET**, **Entity Framework**, and **PostgreSQL** for data storage.

## Features

- **CRUD Operations**: Full Create, Read, Update, and Delete functionalities for managing orders.
- **Data Filtering**: Filter orders by `FIRSTDELIVERYTIME` and `DISTRICT`.
- **Logging**: Logs all processes to track application activity.
- **Data Submission**: Supports sending data to the server in both TXT and JSON formats.

### Sending Data

To send data in TXT format, use this structure, order of parameters does not matter:

Number=order_number;Weight=order_weight;Delivery-Time=delivery_time;District=district_number;

For JSON format, use this structure:

```json
{
    "number": "order_number",
    "weight": 0,
    "deliveryTime": "delivery_time",
    "district": 0
}
```
## Configuration
Before running the application, make sure to set up appsettings.json with these values:

```json
{
  "Paths": {
    "DeliveryLog": "Logs/_deliveryLog.txt",
    "DeliveryOrder": "Logs/_deliveryOrder.txt"
  },
  "ConnectionStrings": {
    "Default": "Your_Connection_String"
  }
}
```
Replace "Your_Connection_String" with the one you use to connect to your database
