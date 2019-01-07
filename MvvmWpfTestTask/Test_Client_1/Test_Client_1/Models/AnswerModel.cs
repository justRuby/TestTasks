using System.Collections.Generic;

namespace Test_Client_1.Models
{
    public class AnswerModel
    {
        public bool Result { get; set; }
        public List<string> errorMessageList { get; set; }

        public AnswerModel()
        {
            Result = false;
            errorMessageList = new List<string>();
        }

    }
}
