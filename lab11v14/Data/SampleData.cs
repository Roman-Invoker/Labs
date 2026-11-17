using lab17v14.Models;

namespace lab17v14.Data;

public static class SampleData
{
    public static List<Recipe> CreateRecipes()
    {
        // Постачальники (8)
        var greenGrove = new Supplier { Id = 1, Name = "Ферма «Зелений Гай»" };
        var mill       = new Supplier { Id = 2, Name = "Млин «Золоте Зерно»" };
        var dairy      = new Supplier { Id = 3, Name = "Молочарня «Буренка»" };
        var niva       = new Supplier { Id = 4, Name = "Агрохолдинг «Нива»" };
        var delta      = new Supplier { Id = 5, Name = "Рибний двір «Дельта»" };
        var hetman     = new Supplier { Id = 6, Name = "М'ясокомбінат «Гетьман»" };
        var apiary     = new Supplier { Id = 7, Name = "Пасіка «Липа»" };
        var spices     = new Supplier { Id = 8, Name = "Спеції Сходу" };

        static Ingredient Ing(int id, string name, params Supplier[] suppliers) =>
            new() { Id = id, Name = name, Suppliers = suppliers.ToList() };

        // Інгредієнти (18): деякі мають кількох постачальників, постачальники повторюються
        var flour    = Ing(1,  "Борошно",        mill, niva);
        var sugar    = Ing(2,  "Цукор",          niva);
        var eggs     = Ing(3,  "Яйця",           greenGrove, niva);
        var milk     = Ing(4,  "Молоко",         dairy);
        var butter   = Ing(5,  "Масло вершкове", dairy, greenGrove);
        var salt     = Ing(6,  "Сіль",           spices);
        var tomatoes = Ing(7,  "Томати",         greenGrove);
        var cheese   = Ing(8,  "Сир",            dairy);
        var chicken  = Ing(9,  "Куряче філе",    hetman);
        var potatoes = Ing(10, "Картопля",       greenGrove, niva);
        var salmon   = Ing(11, "Лосось",         delta);
        var honey    = Ing(12, "Мед",            apiary);
        var cinnamon = Ing(13, "Кориця",         spices);
        var garlic   = Ing(14, "Часник",         greenGrove, spices);
        var pork     = Ing(15, "Свинина",        hetman);
        var beet     = Ing(16, "Буряк",          greenGrove);
        var cabbage  = Ing(17, "Капуста",        greenGrove, niva);
        var apples   = Ing(18, "Яблука",         greenGrove, niva);

        static Recipe Rec(int id, string name, params Ingredient[] ingredients) =>
            new() { Id = id, Name = name, Ingredients = ingredients.ToList() };

        // Рецепти (12)
        return new List<Recipe>
        {
            Rec(1,  "Млинці",                  flour, eggs, milk, sugar, salt),
            Rec(2,  "Борщ",                    beet, cabbage, potatoes, pork, salt, garlic),
            Rec(3,  "Піца Маргарита",          flour, tomatoes, cheese, salt),
            Rec(4,  "Курячий суп",             chicken, potatoes, salt),
            Rec(5,  "Медовик",                 flour, honey, eggs, butter, sugar),
            Rec(6,  "Печений лосось",          salmon, garlic, butter, salt),
            Rec(7,  "Деруни",                  potatoes, eggs, flour, salt),
            Rec(8,  "Сирники",                 cheese, eggs, sugar, flour),
            Rec(9,  "Яблучний пиріг",          flour, sugar, eggs, butter, cinnamon, apples),
            Rec(10, "Омлет",                   eggs, milk, salt, butter),
            Rec(11, "Голубці",                 cabbage, pork, salt, garlic),
            Rec(12, "Томатний крем-суп",       tomatoes, milk, butter, garlic),
        };
    }
}
