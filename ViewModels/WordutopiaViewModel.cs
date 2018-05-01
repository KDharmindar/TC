using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tutioncloud.Models;

namespace tutioncloud.ViewModels
{
    public class WordutopiaViewModel
    {
        #region Constructors

        public WordutopiaViewModel()
        {
            ListOfPreviousWords = new List<int>();
        }

        public WordutopiaViewModel(Wordutopia model): this()
        {

            this.WordID = model.WordID;
            this.FWUserID = model.FWUserID;
            this.Word = model.Word;
            this.Pronunciation = model.Pronounciation;
            this.Phrase = model.Phrase;
            this.Hint = model.Hint;
            this.Definition = model.Definition;
            this.Synonym = model.Synonym;
            //if (ListOfPreviousWords!=null && !ListOfPreviousWords.Contains(model.WordID))
            //{
            //    ListOfPreviousWords.Add(model.WordID);
            //}

        }

        #endregion

        #region Properties

        public int WordID { get; set; }

        public int? FWUserID { get; set; }

        public string Word { get; set; }

        public string Pronunciation { get; set; }

        public string Phrase { get; set; }

        public string Hint { get; set; }

        public string Definition { get; set; }

        public string Synonym { get; set; }

        public List<int> ListOfPreviousWords { get; set; }

         #endregion
    }
}