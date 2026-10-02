using System;

namespace Lab6V14
{
    /// <summary>
    /// Базовий клас ієрархії — узагальнений електронний пристрій.
    /// </summary>
    public class Electronic
    {
        private string brand;
        private int powerConsumption;

        public string Brand
        {
            get { return brand; }
            set { brand = value; }
        }

        public int PowerConsumption
        {
            get { return powerConsumption; }
            set { powerConsumption = value; }
        }

        public Electronic(string brand, int powerConsumption)
        {
            this.brand = brand;
            this.powerConsumption = powerConsumption;
        }

        // Віртуальний метод — похідні класи можуть надати власну реалізацію (override)
        public virtual void TurnOn()
        {
            Console.WriteLine($"[Electronic] Пристрій {brand} вмикається. Споживана потужність: {powerConsumption} Вт.");
        }

        // Звичайний (НЕ віртуальний) метод. Саме його приховає клас Television
        // за допомогою модифікатора new.
        public string GetElectronicType()
        {
            return "Електронний пристрій загального типу";
        }
    }
}
