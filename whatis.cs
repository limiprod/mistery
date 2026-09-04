using System;
namespace WhatIsEngine
{
    public struct WhatIs
    {
        public string Texto { get; set; }

        public WhatIs(string texto) : this()
        {
            Texto = texto;
        }
        public static WhatIs operator +(WhatIs w, WhatIs w2)
        {
            return new WhatIs("positive");
        }
        public static WhatIs operator -(WhatIs w, WhatIs w2)
        {
            return new WhatIs("negative");
        }
        public static WhatIs operator *(WhatIs w1, WhatIs w2)
        {
            return new WhatIs("positive positive");
        }
        public static WhatIs operator /(WhatIs w1, WhatIs w2)
        {
            return new WhatIs("negative negative");
        }
        public override string ToString()
        {
            return Texto;
        }
    }
}