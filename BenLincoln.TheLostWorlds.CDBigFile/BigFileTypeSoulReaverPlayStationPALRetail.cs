// BenLincoln.TheLostWorlds.CDBigFile
// Copyright 2006-2019 Ben Lincoln
// https://www.thelostworlds.net/
//

// This file is part of BenLincoln.TheLostWorlds.CDBigFile.

// BenLincoln.TheLostWorlds.CDBigFile is free software: you can redistribute it and/or modify
// it under the terms of version 3 of the GNU General Public License as published by
// the Free Software Foundation.

// BenLincoln.TheLostWorlds.CDBigFile is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.

// You should have received a copy of the GNU General Public License
// along with BenLincoln.TheLostWorlds.CDBigFile (in the file LICENSE.txt).  
// If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Text;

namespace BenLincoln.TheLostWorlds.CDBigFile
{
    public class BigFileTypeSoulReaverPlayStationPALRetail : BigFileTypeSoulReaverPlayStation
    {
        public BigFileTypeSoulReaverPlayStationPALRetail()
            : base()
        {
            Name = "SoulReaverPlayStationPALRetail";
            Description = "Soul Reaver (PlayStation - PAL English Retail - XOR Encryption Key 0xB722)";
            MasterIndexType = IndexType.SR1PS1PALRetailEnglishMainIndex;
        }
    }
}
