using System;

namespace Lab6V14
{
    /// <summary>
    /// Похідний клас — телевізор.
    /// </summary>
    public class Television : Electronic
    {
        private string screenResolution;

        public string ScreenResolution
        {
            get { return screenResolution; }
            set { screenResolution = value; }
        }

        public Television(string brand, int powerConsumption, string screenResolution)
            : base(brand, powerConsumption)
        {
            this.screenResolution = screenResolution;
        }

        // Перевизначення віртуального методу базового класу
        public override void TurnOn()
        {
            Console.WriteLine($"[Television] Телевізор {Brand} вмикається. Роздільна здатність екрана: {screenResolution}. Споживання: {PowerConsumption} Вт.");
        }

        // Власний унікальний метод похідного класу
        public void ChangeChannel()
        {
            Console.WriteLine($"[Television] Канал на телевізорі {Brand} перемкнуто.");
        }

        // Демонстрація приховування (new): цей метод НЕ перевизначає
        // GetElectronicType() з Electronic, а створює окремий, незалежний
        // метод із тим самим іменем. Який саме метод викликається,
        // залежить від типу ЗМІННОЇ (посилання), а не від реального типу об'єкта.
        public new string GetElectronicType()
        {
            return "Телевізор";
        }
    }
}
