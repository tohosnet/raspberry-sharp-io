namespace Raspberry.IO.GeneralPurpose
{
    /// <summary>
    /// Represents a bidirectional pin on GPIO interface.
    /// </summary>
    public class GpioInputOutputBinaryPin : IInputOutputBinaryPin
    {
        #region Fields

        private readonly IGpioConnectionDriver driver;
        private readonly ProcessorPin pin;
        private readonly PinResistor resistor;
        private PinDirection? direction;

        #endregion

        #region Instance Management

        /// <summary>
        /// Initializes a new instance of the <see cref="GpioInputOutputBinaryPin"/> class.
        /// </summary>
        /// <param name="driver">The driver.</param>
        /// <param name="pin">The pin.</param>
        /// <param name="resistor">The resistor.</param>
        public GpioInputOutputBinaryPin(IGpioConnectionDriver driver, ProcessorPin pin, PinResistor resistor = PinResistor.None)
        {
            this.driver = driver;
            this.pin = pin;
            this.resistor = resistor;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        public void Dispose()
        {
            if (direction.HasValue)
                driver.Release(pin);
        }

        /// <summary>
        /// Reads this instance.
        /// </summary>
        /// <returns>The value.</returns>
        public bool Read()
        {
            SetDirection(PinDirection.Input);
            return driver.Read(pin);
        }

        /// <summary>
        /// Prepares the pin to act as an input.
        /// </summary>
        public void AsInput()
        {
            SetDirection(PinDirection.Input);
        }

        /// <summary>
        /// Prepares the pin to act as an output.
        /// </summary>
        public void AsOutput()
        {
            SetDirection(PinDirection.Output);
        }

        /// <summary>
        /// Waits the specified wait for up.
        /// </summary>
        /// <param name="waitForUp">if set to <c>true</c> [wait for up].</param>
        /// <param name="timeout">The timeout.</param>
        public void Wait(bool waitForUp = true, decimal timeout = 0)
        {
            SetDirection(PinDirection.Input);
            driver.Wait(pin, waitForUp, timeout);
        }

        /// <summary>
        /// Writes the specified state.
        /// </summary>
        /// <param name="state">the state.</param>
        public void Write(bool state)
        {
            SetDirection(PinDirection.Output);
            driver.Write(pin, state);
        }

        #endregion

        #region Private Helpers

        private void SetDirection(PinDirection newDirection)
        {
            if (direction == newDirection)
                return;

            if (direction.HasValue)
                driver.Release(pin);

            driver.Allocate(pin, newDirection);
            if (newDirection == PinDirection.Input
                && resistor != PinResistor.None
                && (driver.GetCapabilities() & GpioConnectionDriverCapabilities.CanSetPinResistor) != GpioConnectionDriverCapabilities.None)
                driver.SetPinResistor(pin, resistor);

            direction = newDirection;
        }

        #endregion
    }
}