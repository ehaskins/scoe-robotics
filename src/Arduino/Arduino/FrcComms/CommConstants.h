/*
 * CommConstants.h
 *
 *  Created on: Apr 4, 2011
 *      Author: EHaskins
 */

#ifndef COMMCONSTANTS_H_
#define COMMCONSTANTS_H_
	//These settings are shared between ALL robot projects.
	//If you're not on the network team, DO NOT TOUCH!
	/*
	//FRC Size
	const int USER_STATUS_DATA_SIZE = 984;
	const int USER_CONTROL_DATA_SIZE = 936;
	*/
	//Tiny

	const int PACKET_SIZE = 192;

	//CONTROL DATA
	const int NUM_JOYSTICKS = 4;
	const static int CONTROL_HEADER_SIZE = 80;
	const static int CONTROL_PACKET_SIZE = PACKET_SIZE;
	const static int USER_CONTROL_DATA_SIZE = CONTROL_PACKET_SIZE - CONTROL_HEADER_SIZE - 8;

	//STATUS DATA
	const static int STATUS_HEADER_SIZE = 32;
	const static int STATUS_PACKET_SIZE = PACKET_SIZE;
	const static int USER_STATUS_DATA_SIZE = STATUS_PACKET_SIZE - STATUS_HEADER_SIZE - 8;

#endif /* COMMCONSTANTS_H_ */
