using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FlaskeAutomat
{
    class Producer
    {
        // event that fires when a bottle gets added to the buffer.
        public event EventHandler OnBottleAdded;
        // main buffer that we send our flasks through first.
        public Bottle[] Flasks { get; set; }
        private bool addedBottle;
        public bool AddedBottle
        {
            get { return addedBottle; }
            set
            {
                if (addedBottle != value)
                {
                    addedBottle = value;
                    TriggerOnBottleAdded();
                }
            }
        }

        private Random rand = new Random();
        private Bottle tempBottle;
        private bool trigger;
        public Producer(Bottle[] flasks)
        {
            this.Flasks = flasks;
        }

        public void Update()
        {
            while (true)
            {
                AddToBuffer();
                Thread.Sleep(3000);
            }
        }

        /// <summary>
        /// Adds a random bottle to our buffer.
        /// </summary>
        private void AddToBuffer()
        {
            for (int i = 0; i < Flasks.Length; i++)
            {
                if (Flasks[i] == null)
                {
                    Monitor.Enter(this);
                    try
                    {
                        int enumeLength = Enum.GetNames(typeof(BottleType)).Length;
                        Flasks[i] = new Bottle(i, (BottleType)rand.Next(0, enumeLength));
                        tempBottle = Flasks[i];
                        AddedBottle = !AddedBottle;
                        trigger = true;
                    }
                    finally
                    {
                        Monitor.PulseAll(this);
                    }
                }
                else
                {
                    Monitor.Wait(this);
                    trigger = false;
                }
            }
        }

        /// <summary>
        /// Triggers an event when a bottle was added to the buffer, and sends a string through the sender that we print out in the manager.
        /// </summary>
        private void TriggerOnBottleAdded()
        {
            EventHandler handler = OnBottleAdded;
            if (handler != null)
            {
                if (trigger)
                    OnBottleAdded($"Producer added {tempBottle.Type} to the buffer", EventArgs.Empty);
                else
                    OnBottleAdded($"Producer is waiting...", EventArgs.Empty);
            }
        }
    }
}
