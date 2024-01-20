namespace MabiCooker
{
    public class Recipe
    {
        public string Name { get; private set; }
        public int Ingredient1 { get; private set; }
        public int Ingredient2 { get; private set; }
        public int Ingredient3 { get; private set; }

        public Recipe(string name, int ingredient1, int ingredient2, int ingredient3)
        {
            Name = name;
            Ingredient1 = ingredient1;
            Ingredient2 = ingredient2;
            Ingredient3 = ingredient3;
        }
    }
}