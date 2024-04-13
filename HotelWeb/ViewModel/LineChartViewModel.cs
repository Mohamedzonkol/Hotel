namespace HotelWeb.ViewModel
{
    public class LineChartViewModel
    {
        public List<ChartData> Series { get; set; }
        public string[] Categorie { get; set; }
    }
    public class ChartData
    {
        public string Name { get; set; }
        public int[] Data { get; set; }

    }
}
