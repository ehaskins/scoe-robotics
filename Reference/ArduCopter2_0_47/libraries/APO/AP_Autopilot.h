/*
 * AP_Autopilot.h
 *
 *  Created on: Apr 30, 2011
 *      Author: jgoppert
 */

#ifndef AP_AUTOPILOT_H_
#define AP_AUTOPILOT_H_

/*
 * AVR runtime
 */
#include <avr/io.h>
#include <avr/eeprom.h>
#include <avr/pgmspace.h>
#include <math.h>
/*
 * Libraries
 */
#include "../AP_Common/AP_Common.h"
#include "../FastSerial/FastSerial.h"
#include "../AP_GPS/GPS.h"
#include "../APM_RC/APM_RC.h"
#include "../AP_ADC/AP_ADC.h"
#include "../APM_BMP085/APM_BMP085.h"
#include "../AP_Compass/AP_Compass.h"
#include "../AP_Math/AP_Math.h"
#include "../AP_IMU/AP_IMU.h"
#include "../AP_DCM/AP_DCM.h"
#include "../AP_Common/AP_Loop.h"
#include "../GCS_MAVLink/GCS_MAVLink.h"
#include "../AP_RangeFinder/AP_RangeFinder.h"
/*
 * Local Modules
 */
#include "AP_HardwareAbstractionLayer.h"
#include "AP_RcChannel.h"
#include "AP_Controller.h"
#include "AP_Navigator.h"
#include "AP_Guide.h"
#include "AP_CommLink.h"

/**
 * ArduPilotOne namespace to protect variables
 * from overlap with avr and libraries etc.
 * ArduPilotOne does not use any global
 * variables.
 */
namespace apo {

// forward declarations
class AP_CommLink;

/**
 * This class encapsulates the entire autopilot system
 * The constructor takes guide, navigator, and controller
 * as well as the hardware abstraction layer.
 *
 * It inherits from loop to manage
 * the sub-loops and sets the overall
 * frequency for the autopilot.
 *

 */
class AP_Autopilot: public Loop {
public:
	/**
	 * Default constructor
	 */
	AP_Autopilot(AP_Navigator * navigator, AP_Guide * guide,
			AP_Controller * controller, AP_HardwareAbstractionLayer * hal,
			float loop0Rate, float loop1Rate, float loop2Rate, float loop3Rate);

	/**
	 * Accessors
	 */
	AP_Navigator * getNavigator() {
		return _navigator;
	}
	AP_Guide * getGuide() {
		return _guide;
	}
	AP_Controller * getController() {
		return _controller;
	}
	AP_HardwareAbstractionLayer * getHal() {
		return _hal;
	}

	float getLoopRate(uint8_t i) {
		switch(i) {
		case 0: return _loop0Rate;
		case 1: return _loop1Rate;
		case 2: return _loop2Rate;
		case 3: return _loop3Rate;
		case 4: return _loop4Rate;
		default: return 0;
		}
	}

private:

	/**
	 * Loop 0 Callbacks (fastest)
	 * - inertial navigation
	 * @param data A void pointer used to pass the apo class
	 *  so that the apo public interface may be accessed.
	 */
	static void callback0(void * data);
	float _loop0Rate;

	/**
	 * Loop 1 Callbacks
	 * - control
	 * - compass reading
	 * @see callback0
	 */
	static void callback1(void * data);
	float _loop1Rate;

	/**
	 * Loop 2 Callbacks
	 * - gps sensor fusion
	 * - compass sensor fusion
	 * @see callback0
	 */
	static void callback2(void * data);
	float _loop2Rate;

	/**
	 * Loop 3 Callbacks
	 * - slow messages
	 * @see callback0
	 */
	static void callback3(void * data);
	float _loop3Rate;

	/**
	 * Loop 4 Callbacks
	 * - super slow messages
	 * - log writing
	 * @see callback0
	 */
	static void callback4(void * data);
	float _loop4Rate;

	/**
	 * Components
	 */
	AP_Navigator * _navigator;
	AP_Guide * _guide;
	AP_Controller * _controller;
	AP_HardwareAbstractionLayer * _hal;

	/**
	 * Constants
	 */
	static const float deg2rad = M_PI / 180;
	static const float rad2deg = 180 / M_PI;
};

} // namespace apo

#endif /* AP_AUTOPILOT_H_ */
