using Effective_Mobile.Data;
using Effective_Mobile.Models;
using Microsoft.AspNetCore.Mvc;
using static Effective_Mobile.Helpers.Logger;

namespace Effective_Mobile.Controllers
{
    [ApiController]
    [Route("order")]
    public class OrderController(ApplicationContext context) : Controller
    {
        /// <summary>
        /// Converts each line to Order object
        /// Each line should look like "Number=**;Weight=**;District=**;Delivery-Time=**;" 
        /// Order of parameters does not matter.
        /// </summary>
        /// <param name="file">text file with orders information</param>
        [HttpPost]
        [Route("upload-file-txt")]
        public ActionResult AddOrdersFromTxt(IFormFile file)
        {
            using var reader = new StreamReader(file.OpenReadStream());
            string content = reader.ReadToEnd();
            bool allSucceed = ProcessOrders(content);
            context.SaveChanges();


            return allSucceed ? Ok("All orders were added successfully")
                              : BadRequest("Not all orders were added successfully");
        }
        /// <summary>
        /// Tries to add all the Order objects in the list to the database
        /// Id should not be passed, as it may become the reason of conflicts
        /// </summary>
        /// <param name="orders">list of orders</param>
        [HttpPost]
        [Route("upload-file-json")]
        public ActionResult AddOrdersFromJson([FromBody] List<Order> orders)
        {
            int count = 0;
            for (int i = 0; i < orders.Count; i++)
            {
                if (orders[i].Valid())
                {
                    if (context.Orders.Any(order => order.Id == orders[i].Id))
                    {
                        LogWarning($"Trying to add order with id({orders[i].Id}) that's already in database");
                        continue;
                    }

                    LogInformation("Added order with number: " + orders[i].Number);
                    context.Orders.Add(orders[i]);
                    count++;
                }
                else
                    LogError("Wrong format in order " + orders[i].Number);
            }
            context.SaveChanges();
            LogInformation($"Added {count} orders");
            return count == orders.Count ? Ok("All orders were added successfully")
                              : BadRequest("Not all orders were added successfully");
        }
        /// <summary>
        /// Finds the Order in the database with the same id and applies changes
        /// </summary>
        /// <param name="Order">order with new data</param>
        [HttpPatch]
        [Route("update")]
        public ActionResult UpdateOrder([FromForm] Order Order)
        {
            var found = context.Orders.FirstOrDefault(order => order.Id == Order.Id);
            if (found == null)
            {
                LogWarning($"Trying to update non existing order");
                return NotFound("Order with this id was not found");
            }
            found.Number = Order.Number;
            found.District = Order.District;
            found.Weight = Order.Weight;
            found.DeliveryTime = Order.DeliveryTime.ToUniversalTime();

            LogInformation($"Order with id {found.Id} updated");

            context.SaveChanges();

            return Ok("Order updated");
        }

        /// <summary>
        /// Deletes the order with received Id
        /// </summary>
        /// <param name="id">Order id</param>
        [HttpDelete]
        [Route("delete")]
        public ActionResult Delete([FromForm] int id)
        {
            var found = context.Orders.FirstOrDefault(order => order.Id == id);
            if (found == null)
            {
                LogWarning($"Trying to delete non existing order");
                return NotFound("Order with this id was not found");
            }
            context.Orders.Remove(found);
            context.SaveChanges();

            LogInformation($"Order with id {id} deleted");

            return Ok("Order deleted");
        }

        /// <summary>
        /// Finds the order with received id
        /// </summary>
        /// <param name="id">Order id</param>
        /// <returns>Found order or NotFound error</returns>
        [HttpGet]
        [Route("get")]
        public ActionResult Get(int id)
        {
            var found = context.Orders.FirstOrDefault(order => order.Id == id);
            if (found == null)
            {
                LogWarning($"Requested non existing order");
                return NotFound("Order with this id was not found");
            }

            LogInformation($"Order with id {id} was requested");
            LogOrder(found.ToString());
            return Ok(found);
        }
        /// <returns>All existing orders in database</returns>
        [HttpGet]
        [Route("all")]
        public ActionResult All()
        {
            LogInformation($"All orders were requested");
            var orders = context.Orders.ToArray();
            for (int i = 0; i < orders.Length; i++)
                LogOrder(orders[i].ToString());

            return Ok(orders);
        }
        /// <summary>
        /// Checks all existing orders in database and returns the ones which district is equal to recieved one 
        /// and delivery date is between firstDeliveryDate and firstDeliveryDate + 30min
        /// If district is not passed in request, orders from all the district will be considered
        /// </summary>
        /// <param name="firstDeliveryDate">Date</param>
        /// <param name="district">District id</param>
        /// <returns>Relevant Orders</returns>
        [HttpPost]
        [Route("filtered")]
        public ActionResult Filtered([FromForm] DateTime firstDeliveryDate, [FromForm] int district = -1)
        {
            if (firstDeliveryDate == DateTime.MinValue)
            {
                LogWarning("Wrong date for filtered orders");
                return BadRequest("You did not entered first delivery date");
            }
            firstDeliveryDate = firstDeliveryDate.ToUniversalTime();
            var orders = context.Orders.Where(order => (district < 0 || order.District == district)
                                       && (order.DeliveryTime >= firstDeliveryDate
                                       && order.DeliveryTime <= firstDeliveryDate.AddHours(0.5)));

            LogInformation($"Orders with filters date: {firstDeliveryDate} "
                                        + (district > 0 ? $"and district: {district} " : "")
                                        + "were requested");

            foreach (var order in orders)
                LogOrder(order.ToString());

            return Ok(orders);
        }
        /// <summary>
        /// Helper method for "AddOrdersFromTxt". Converts each line into Order object and adds valid ones to the database
        /// </summary>
        /// <returns>True if all lines were converted right</returns>
        private bool ProcessOrders(string content)
        {
            bool allSucceeded = true;
            int count = 0;
            foreach (var orderLine in content.Split('\n').Select(line => line.Trim()))
            {

                if (orderLine == "")
                {
                    LogWarning("Trying to pass an empty string");
                    continue;
                }
                try
                {
                    var order = Order.FromLine(orderLine);

                    if (order.Valid())
                    {
                        LogInformation("Added order with number: " + order.Number);
                        context.Orders.Add(order);
                        count++;
                    }
                    else
                        LogWarning("Empty values passed: " + orderLine);
                }
                catch
                {
                    LogError($"Wrong format in order line: {orderLine}");
                    allSucceeded = false;
                }
            }
            LogInformation($"Added {count} orders");
            return allSucceeded;
        }
    }
}
