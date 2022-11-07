using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace todo_aspnetmvc_ui.Infrastructure
{
    public class Class
    {
        Dictionary<char, int> _numsValues;

        public Class()
        {
            _numsValues = new Dictionary<char, int>
            {
                {'I', 1 },
                {'V', 5 },
                {'X', 10 },
                {'L', 50 },
                {'C', 100 },
                {'D', 500 },
                {'M', 1000 }
            };
        }

        public int RomanToInt(string romanNumber)
        {
            var intNum = 0;
            var prevSym = 'M';

            foreach (var sym in romanNumber)
            {
                if(sym == 'V' || sym == 'X')
                {
                    intNum += prevSym == 'I' ? (_numsValues[sym]-2) : _numsValues[sym];
                }
                else if (sym == 'L' || sym == 'C')
                {
                    intNum += prevSym == 'X' ? (_numsValues[sym] - 20) : _numsValues[sym];
                }
                else if (sym == 'D' || sym == 'M')
                {
                    intNum += prevSym == 'C' ? (_numsValues[sym] - 200) : _numsValues[sym];
                }
                else
                {
                    intNum += _numsValues[sym];
                }

                prevSym = sym;
            }

            return intNum;
        }
    }
}
