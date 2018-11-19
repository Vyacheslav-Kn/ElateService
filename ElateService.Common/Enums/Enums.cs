using System.Collections.Generic;

namespace ElateService.Common
{
    public enum Role
    {
        Customer,
        Executor
    }

    public enum Category
    {
        ConstructionWorks,
        TransportationServices,
        HouseWorks,
        ComputerHelp,
        Sport,
        Education,
        Tourism
    }

    public static class CategoryExtension
    {
        public static string[] TranslateFromEnumToRussianEquivalents(string[] categoriesEn)
        {
            if(categoriesEn == null)
            {
                return null;
            }

            for (int i = 0; i < categoriesEn.Length; i++)
            {
                switch (categoriesEn[i])
                {
                    case "ConstructionWorks": categoriesEn[i] = "Строительные работы"; break;

                    case "TransportationServices": categoriesEn[i] = "Перевозки"; break;

                    case "ComputerHelp": categoriesEn[i] = "Компьютерная помощь"; break;

                    case "HouseWorks": categoriesEn[i] = "Все для дома"; break;

                    case "Sport": categoriesEn[i] = "Спорт"; break;

                    case "Education": categoriesEn[i] = "Образование"; break;

                    case "Tourism": categoriesEn[i] = "Туризм"; break;

                    default: categoriesEn[i] = null; break;
                }
            }
            return categoriesEn;
        }
         

        public static List<Category> TranslateFromRussianToEnumEquivalents(string[] categoriesRus)
        {
            if (categoriesRus == null)
            {
                return null;
            }

            List<Category> categories = new List<Category>();

            for (int i = 0; i < categoriesRus.Length; i++)
            {
                switch (categoriesRus[i])
                {
                    case "Строительные работы": categories.Add(Category.ConstructionWorks); break;

                    case "Перевозки": categories.Add(Category.TransportationServices); break;

                    case "Компьютерная помощь": categories.Add(Category.ComputerHelp); break;

                    case "Все для дома": categories.Add(Category.HouseWorks); break;

                    case "Спорт": categories.Add(Category.Sport); break;

                    case "Образование": categories.Add(Category.Education); break;

                    case "Туризм": categories.Add(Category.Tourism); break;
                }
            }
            return categories;
        }


        public static string[] TranslateFromEnumToEnglishEquivalents(string[] categoriesEnumInEngl)
        {
            if (categoriesEnumInEngl == null)
            {
                return null;
            }

            for (int i = 0; i < categoriesEnumInEngl.Length; i++)
            {
                switch (categoriesEnumInEngl[i])
                {
                    case "ConstructionWorks": categoriesEnumInEngl[i] = "Construction works"; break;

                    case "TransportationServices": categoriesEnumInEngl[i] = "Transportation services"; break;

                    case "ComputerHelp": categoriesEnumInEngl[i] = "Computer help"; break;

                    case "HouseWorks": categoriesEnumInEngl[i] = "House works"; break;

                    case "Sport": categoriesEnumInEngl[i] = "Sport"; break;

                    case "Education": categoriesEnumInEngl[i] = "Education"; break;

                    case "Tourism": categoriesEnumInEngl[i] = "Tourism"; break;

                    default: categoriesEnumInEngl[i] = null; break;
                }
            }
            return categoriesEnumInEngl;
        }


        public static List<Category> TranslateFromEnglishToEnumEquivalents(string[] categoriesInEngl)
        {
            if (categoriesInEngl == null)
            {
                return null;
            }

            List<Category> categories = new List<Category>();

            for (int i = 0; i < categoriesInEngl.Length; i++)
            {
                switch (categoriesInEngl[i])
                {
                    case "Construction works": categories.Add(Category.ConstructionWorks); break;

                    case "Transportation services": categories.Add(Category.TransportationServices); break;

                    case "Computer help": categories.Add(Category.ComputerHelp); break;

                    case "House works": categories.Add(Category.HouseWorks); break;

                    case "Sport": categories.Add(Category.Sport); break;

                    case "Education": categories.Add(Category.Education); break;

                    case "Tourism": categories.Add(Category.Tourism); break;
                }
            }
            return categories;
        }

    }
}
