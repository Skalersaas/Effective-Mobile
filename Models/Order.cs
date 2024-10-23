using static Effective_Mobile.Helpers.Parser;

namespace Effective_Mobile.Models
{
    public partial class Order
    {
        public int Id { get; set; }
        public string Number { get; set; }
        public int Weight { get; set; }
        public int District { get; set; }
        public DateTime DeliveryTime { get; set; }

        /// <summary>
        /// Create Order object from line, extracting every needed parameters
        /// </summary>
        /// <param name="line">string to extract from</param>
        /// <returns>new Object with those parameters</returns>
        public static Order FromLine(string line)
        {
            return new Order
            {
                Number = Extract(line, "Number"),
                Weight = int.Parse(Extract(line, "Weight")),
                District = int.Parse(Extract(line, "District")),
                DeliveryTime = ParseDeliveryTime(Extract(line, "Delivery-Time"))
            };
        }
        /// <summary>
        /// Checks if all needed fields are filled
        /// </summary>
        public bool Valid()
        {
            return (Number != "" && Weight != -1 && District != -1 && DeliveryTime != DateTime.MinValue);
        }
        public override string ToString()
        {
            return $"Id:{Id};Number:{Number};Weight:{Weight};District:{District};Delivery-Time:{DeliveryTime}";
        }
    }
}
