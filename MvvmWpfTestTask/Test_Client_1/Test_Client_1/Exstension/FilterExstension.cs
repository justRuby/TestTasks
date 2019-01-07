using System;
using System.Linq;

namespace Test_Client_1.Exstension
{
    using Test_Client_1.Models;

    public static class FilterExstension
    {
        public static bool Filter<T>(this T obj, string compared, Type objType)
        {
            bool result = false;

            if(objType == typeof(NoteModel))
            {
                var note = (obj as NoteModel);

                if (note.Equals(compared))
                    return result;

                result = (note as NoteModel).Headline.Where((xN) =>
                {
                    foreach (var xC in compared)
                    {
                        if (xN == xC)
                            return true;
                    }

                    return false;
                }).Any();
            }

            return result;
        }
    }
}
