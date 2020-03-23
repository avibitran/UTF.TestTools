using System;

namespace UTF.TestTools.DAL
{
    public interface IRestMessage
    {
        string ToString();

        void Deserialize(string value);

        void SetContent();
    }
}
