/*
 * PID.h
 *
 *  Created on: Feb 24, 2011
 *      Author: Nick Iannone
 */

#ifndef PID_H_
#define PID_H_

#include "../UserCode.h"

// The number of integral segments to track in the PID control.
// Can be overridden in UserCode.h.
#ifndef PID_NSEGS
	#define PID_NSEGS	10
#endif

// The scaling factor for the BCD.
// Can be overridden in UserCode.h.
#ifndef PID_SCALE
	#define PID_SCALE	128		// Allows values between -256.0 and 255.9921875.
#endif

/**
 * Represents a standard PID control.
 *
 * Implemented via a template class so the compiler can optimize code at runtime.
 * This implementation is meant to work with signed 16-bit BCD numbers.
 *
 * @see http://en.wikipedia.org/wiki/PID_controller
 */
template <typename T = int16_t>
class PID {
protected:
	/**
	 * The three gain coefficients for the loop.
	 */
	T kP, kI, kD;

	/**
	 * The array of BCD error values.
	 */
	T prevErrors[PID_NSEGS];

	/**
	 * The array of BCD delta-time values.
	 */
	T prevTimes[PID_NSEGS];

	/**
	 * The current offset into prevErrors.
	 */
	int errIndex;

	/**
	 * Performs a three-step multiply-divide, maintaining lost decimal digits.
	 */
	static T mulDiv(T value, T mul, T div) {
		T prod = value / div;
		T rem = value % div;
		return (prod * mul) + ((rem * mul) / div);
	}

	/**
	 * Called to feed the list of error values.
	 */
	void trackOutput(T output, T timespan) {
		// Store the current values.
		prevErrors[errIndex] = output;
		prevTimes[errIndex] = timespan;
		// Increment the error index.
		errIndex = (errIndex + 1) % PID_NSEGS;
	}

	T getLastError() {
		int index = (errIndex == 0) ? (PID_NSEGS) : (errIndex);
		return prevErrors[index - 1];
	}

	T getDerivative(T current, T cycleTime) {
		T d = 0;
		if (cycleTime != 0) {
			T offset = current - getLastError();
			d = mulDiv(offset, kD, cycleTime);
		}
		return d;
	}

	T getIntegral() {
		T sum = 0;
		for (int i = 0; i < PID_NSEGS; i++) {
			T error = prevErrors[i];
			T timestep = prevTimes[i];
			sum += mulDiv(error, kI * timeScale, PID_SCALE);
		}
		return sum;
	}

public:
	PID(T p, T i, T d) {
		kP = p;
		kI = i;
		kD = d;
		errIndex = 0;
		for (int i = 0; i < PID_NSEGS; i++) {
			prevErrors[i] = 0;
			prevTimes[i] = 1;
		}
	}
	~PID() {}

	/**
	 * A utility to auto-tune the gain coefficients using the Ziegler-Nichols method.
	 * TODO Implement this!
	 */
	void autoTune() {

	}

	void setKP(T p) {
		kP = p;
	}
	T getKP() {
		return kP;
	}

	void setKI(T i) {
		kI = i;
	}
	T getKI() {
		return kI;
	}

	void setKD(T d) {
		kD = d;
	}
	T getKD() {
		return kD;
	}

	/**
	 * The PID control method. Accepts inputs setPoint and actual, and a period time.
	 */
	T filter(T setPoint, T actual, T cycleTime = 1) {

		// Instantaneous error.
		T error = (setPoint - actual);

		// Calculate the proportional value.
		T p = kP * error;

		// Calculate the derivative value.
		T d = getDerivative(error, cycleTime);

		// Calculate the integral value.
		T i = getIntegral();

		// Generate the output.
		T output = p + i + d;

		// Store the output.
		trackOutput(output, cycleTime);

		// Return the sum.
		return output;
	}
};

#endif /* PID_H_ */
