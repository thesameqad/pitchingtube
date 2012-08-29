using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PitchingTube.Data
{
    public class TubeRepository:BaseRepository<Tube>
    {
        public void ChangeMode(Tube tube, TubeMode tubeMode)
        {
            tube.TubeMode = tubeMode;
            Update(tube);
        }

        public int GetTubeTimeout(int tubeId)
        {
            var currentTube = FirstOrDefault(t => t.TubeId == tubeId);

            TimeSpan timeDiff = DateTime.Now - currentTube.CreatedDate;

            return Convert.ToInt32(timeDiff.TotalMinutes);
        }

    }
}
