using System;

namespace Lab6V14
{
    /// <summary>
    /// Похідний клас — радіоприймач.
    /// </summary>
    public class Radio : Electronic
    {
        private string frequencyRange;

        public string FrequencyRange
        {
            get { return frequencyRange; }
            set { frequencyRange = value; }
        }

        public Radio(string brand, int powerConsumption, string frequencyRange)
            : base(brand, powerConsumption)
        {
            this.frequencyRange = frequencyRange;
        }

        // Перевизначення віртуального методу базового класу
        public override void TurnOn()
        {
            Console.WriteLine($"[Radio] Радіоприймач {Brand} вмикається. Діапазон частот: {frequencyRange}. Споживання: {PowerConsumption} Вт.");
        }

        // Власний унікальний метод похідного класу
        public void TuneStation()
        {
            Console.WriteLine($"[Radio] Радіоприймач {Brand} налаштовано на нову станцію.");
        }
    }
}
