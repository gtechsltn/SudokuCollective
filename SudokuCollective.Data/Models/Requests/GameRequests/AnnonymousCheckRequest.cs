using System.Collections.Generic;
using System.Linq;
using SudokuCollective.Core.Interfaces.Models.DomainObjects.Requests;

namespace SudokuCollective.Data.Models.Requests
{
    public class AnnonymousCheckRequest : IAnnonymousCheckRequest
    {
        public List<int> FirstRow { get; set; }
        public List<int> SecondRow { get; set; }
        public List<int> ThirdRow { get; set; }
        public List<int> FourthRow { get; set; }
        public List<int> FifthRow { get; set; }
        public List<int> SixthRow { get; set; }
        public List<int> SeventhRow { get; set; }
        public List<int> EighthRow { get; set; }
        public List<int> NinthRow { get; set; }

        public AnnonymousCheckRequest()
        {
            FirstRow = new List<int>();
            SecondRow = new List<int>();
            ThirdRow = new List<int>();
            FourthRow = new List<int>();
            FifthRow = new List<int>();
            SixthRow = new List<int>();
            SeventhRow = new List<int>();
            EighthRow = new List<int>();
            NinthRow = new List<int>();
        }

        public AnnonymousCheckRequest(
            int[] firstRow, 
            int[] secondRow, 
            int[] thirdRow, 
            int[] fourthRow, 
            int[] fifthRow, 
            int[] sixthRow, 
            int[] seventhRow, 
            int[] eighthRow, 
            int[] ninthRow)
        {
            FirstRow = firstRow.ToList();
            SecondRow = secondRow.ToList();
            ThirdRow = thirdRow.ToList();
            FourthRow = fourthRow.ToList();
            FifthRow = fifthRow.ToList();
            SixthRow = sixthRow.ToList();
            SeventhRow = seventhRow.ToList();
            EighthRow = eighthRow.ToList();
            NinthRow = ninthRow.ToList();
        }

        public AnnonymousCheckRequest(
            List<int> firstRow, 
            List<int> secondRow, 
            List<int> thirdRow, 
            List<int> fourthRow, 
            List<int> fifthRow, 
            List<int> sixthRow, 
            List<int> seventhRow, 
            List<int> eighthRow, 
            List<int> ninthRow)
        {
            FirstRow = firstRow;
            SecondRow = secondRow;
            ThirdRow = thirdRow;
            FourthRow = fourthRow;
            FifthRow = fifthRow;
            SixthRow = sixthRow;
            SeventhRow = seventhRow;
            EighthRow = eighthRow;
            NinthRow = ninthRow;
        }
    }
}