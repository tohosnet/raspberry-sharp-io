#region References

using System;

#endregion

namespace Raspberry.IO.Components.Expanders.Mcp23017
{
    /// <summary>
    /// Represents a binary intput pin on a MCP23017 I/O expander.
    /// </summary>
    public class Mcp23017InputBinaryPin : IInputBinaryPin
    {
        #region Fields

        private readonly Mcp23017I2cConnection connection;
        private readonly Mcp23017Pin pin;

        #endregion

        #region Instance Management

        /// <summary>
        /// Initializes a new instance of the <see cref="Mcp23017InputBinaryPin"/> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="pin">The pin.</param>
        /// <param name="resistor">The resistor.</param>
        /// <param name="polarity">The polarity.</param>
        public Mcp23017InputBinaryPin(Mcp23017I2cConnection connection, Mcp23017Pin pin,
            Mcp23017PinResistor resistor = Mcp23017PinResistor.None,
            Mcp23017PinPolarity polarity = Mcp23017PinPolarity.Normal)
        {
            this.connection = connection;
            this.pin = pin;

            connection.SetDirection(pin, Mcp23017PinDirection.Input);
            connection.SetResistor(pin, resistor);
            connection.SetPolarity(pin, polarity);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose(){}

        #endregion

        #region Methods

        /// <summary>
        /// Reads the state of the pin.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if the pin is in high state; otherwise, <c>false</c>.
        /// </returns>
        public bool Read()
        {
            return connection.GetPinStatus(pin);
        }

        /// <summary>
        /// Waits for the specified pin to be in the specified state.
        /// </summary>
        /// <param name="waitForUp">if set to <c>true</c> waits for the pin to be up.</param>
        /// <param name="timeout">The timeout, in milliseconds.</param>
        /// <exception cref="System.TimeoutException">A timeout occurred while waiting for pin status to change</exception>
        public void Wait(bool waitForUp = true, decimal timeout = 0)
        {
            var startWait = DateTime.Now;
            if (timeout == 0)
                timeout = 5000;

            while (Read() != waitForUp)
            {
                if (DateTime.Now.Ticks - startWait.Ticks >= 10000*timeout)
                    throw new TimeoutException("A timeout occurred while waiting for pin status to change");
            }
        }

        #endregion
    }
}