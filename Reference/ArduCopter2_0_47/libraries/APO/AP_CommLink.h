/*
 * AP_CommLink.h
 * Copyright (C) James Goppert 2010 <james.goppert@gmail.com>
 *
 * This file is free software: you can redistribute it and/or modify it
 * under the terms of the GNU General Public License as published by the
 * Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This file is distributed in the hope that it will be useful, but
 * WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
 * See the GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License along
 * with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

#ifndef AP_CommLink_H
#define AP_CommLink_H

#include "AP_HardwareAbstractionLayer.h"
#include "AP_MavlinkCommand.h"
#include "AP_Controller.h"

namespace apo {

class AP_Controller;
class AP_Navigator;
class AP_Guide;
class AP_HardwareAbstractionLayer;

enum {
	SEVERITY_LOW, SEVERITY_MED, SEVERITY_HIGH
};

// forward declarations
//class ArduPilotOne;
//class AP_Controller;

/// CommLink class
class AP_CommLink {
public:

	AP_CommLink(FastSerial * link, AP_Navigator * navigator, AP_Guide * guide,
			AP_Controller * controller, AP_HardwareAbstractionLayer * hal) :
		_link(link), _navigator(navigator), _guide(guide),
				_controller(controller), _hal(hal) {
	}
	virtual void send() = 0;
	virtual void receive() = 0;
	virtual void sendMessage(uint8_t id, uint32_t param = 0) = 0;
	virtual void sendText(uint8_t severity, const char *str) = 0;
	virtual void sendText(uint8_t severity, const prog_char_t *str) = 0;
	virtual void acknowledge(uint8_t id, uint8_t sum1, uint8_t sum2) = 0;
	virtual void sendParameters() = 0;
	virtual void requestCmds() = 0;

protected:
	FastSerial * _link;
	AP_Navigator * _navigator;
	AP_Guide * _guide;
	AP_Controller * _controller;
	AP_HardwareAbstractionLayer * _hal;
};

class MavlinkComm: public AP_CommLink {
public:
	MavlinkComm(FastSerial * link, AP_Navigator * nav, AP_Guide * guide,
			AP_Controller * controller, AP_HardwareAbstractionLayer * hal) :
				AP_CommLink(link, nav, guide, controller, hal),

				// options
				_useRelativeAlt(true),

				// commands
				_sendingCmds(false), _receivingCmds(false),
				_cmdTimeLastSent(millis()), _cmdTimeLastReceived(millis()),
				_cmdDestSysId(0), _cmdDestCompId(0), _cmdRequestIndex(0),
				_cmdMax(30), _cmdNumberRequested(0),

				// parameters
				_parameterCount(0), _queuedParameter(NULL),
				_queuedParameterIndex(0) {

		switch (_nChannels) {
		case 0:
			mavlink_comm_0_port = link;
			_channel = MAVLINK_COMM_0;
			_nChannels++;
			break;
		case 1:
			mavlink_comm_1_port = link;
			_channel = MAVLINK_COMM_1;
			_nChannels++;
			break;
		default:
			// signal that number of channels exceeded
			_channel = MAVLINK_COMM_3;
			break;
		}
	}

	virtual void send() {
		// if number of channels exceeded return
		if (_channel == MAVLINK_COMM_3)
			return;
	}

	void sendMessage(uint8_t id, uint32_t param = 0) {
		//_hal->debug->printf_P(PSTR("send message\n"));

		// if number of channels exceeded return
		if (_channel == MAVLINK_COMM_3)
			return;

		uint64_t timeStamp = micros();

		switch (id) {

		case MAVLINK_MSG_ID_HEARTBEAT: {
			mavlink_msg_heartbeat_send(_channel, mavlink_system.type,
					MAV_AUTOPILOT_ARDUPILOTMEGA);
			break;
		}

		case MAVLINK_MSG_ID_ATTITUDE: {
			mavlink_msg_attitude_send(_channel, timeStamp,
					_navigator->getRoll(), _navigator->getPitch(),
					_navigator->getYaw(), _navigator->getRollRate(),
					_navigator->getPitchRate(), _navigator->getYawRate());
			break;
		}

		case MAVLINK_MSG_ID_GLOBAL_POSITION: {
			mavlink_msg_global_position_send(_channel, timeStamp,
					_navigator->getLat() * rad2Deg,
					_navigator->getLon() * rad2Deg, _navigator->getAlt(),
					_navigator->getVN(), _navigator->getVE(),
					_navigator->getVD());
			break;
		}

		case MAVLINK_MSG_ID_GPS_RAW: {
			mavlink_msg_gps_raw_send(_channel, timeStamp, _hal->gps->status(),
					_navigator->getLat() * rad2Deg,
					_navigator->getLon() * rad2Deg, _navigator->getAlt(), 0, 0,
					_navigator->getGroundSpeed(),
					_navigator->getYaw() * rad2Deg);
			break;
		}

			/*
			 case MAVLINK_MSG_ID_GPS_RAW_INT: {
			 mavlink_msg_gps_raw_int_send(_channel,timeStamp,_hal->gps->status(),
			 _navigator->getLat_degInt(), _navigator->getLon_degInt(),_navigator->getAlt_intM(), 0,0,
			 _navigator->getGroundSpeed(),_navigator->getYaw()*rad2Deg);
			 break;
			 }
			 */

		case MAVLINK_MSG_ID_SCALED_IMU: {
			/*
			 * accel/gyro debug
			 */
			/*
			 Vector3f accel = _hal->imu->get_accel();
			 Vector3f gyro = _hal->imu->get_gyro();
			 Serial.printf_P(PSTR("accel: %f %f %f gyro: %f %f %f\n"),
			 accel.x,accel.y,accel.z,gyro.x,gyro.y,gyro.z);
			 */
			Vector3f accel = _hal->imu->get_accel();
			Vector3f gyro = _hal->imu->get_gyro();
			mavlink_msg_raw_imu_send(_channel, timeStamp, 1000 * accel.x,
					1000 * accel.y, 1000 * accel.z, 1000 * gyro.x,
					1000 * gyro.y, 1000 * gyro.z, _hal->compass->mag_x,
					_hal->compass->mag_y, _hal->compass->mag_z); // XXX THIS IS NOT SCALED FOR MAG
		}

		case MAVLINK_MSG_ID_RC_CHANNELS_SCALED: {
			int16_t ch[8];
			for (int i = 0; i < 8; i++)
				ch[i] = 0;
			for (uint8_t i = 0; i < 8 && i < _hal->rc.getSize(); i++) {
				ch[i] = 10000 * _hal->rc[i]->getPosition();
				//_hal->debug->printf_P(PSTR("ch: %d position: %d\n"),i,ch[i]);
			}
			mavlink_msg_rc_channels_scaled_send(_channel, ch[0], ch[1], ch[2],
					ch[3], ch[4], ch[5], ch[6], ch[7], 255);
			break;
		}

		case MAVLINK_MSG_ID_RC_CHANNELS_RAW: {
			int16_t ch[8];
			for (int i = 0; i < 8; i++)
				ch[i] = 0;
			for (uint8_t i = 0; i < 8 && i < _hal->rc.getSize(); i++)
				ch[i] = _hal->rc[i]->getPwm();
			mavlink_msg_rc_channels_raw_send(_channel, ch[0], ch[1], ch[2],
					ch[3], ch[4], ch[5], ch[6], ch[7], 255);
			break;
		}

		case MAVLINK_MSG_ID_SYS_STATUS: {

			float batteryVoltage, temp;
			temp = analogRead(0);
			batteryVoltage = ((temp * 5 / 1023) / 0.28);

			mavlink_msg_sys_status_send(_channel, _controller->getMode(),
					_guide->getMode(), _hal->getState(), _hal->load * 10,
					batteryVoltage * 1000,
					(batteryVoltage - 3.3) / (4.2 - 3.3) * 1000, _packetDrops);
			break;
		}

		case MAVLINK_MSG_ID_WAYPOINT_ACK: {
			sendText(SEVERITY_LOW, PSTR("waypoint ack"));
			//mavlink_waypoint_ack_t packet;
			uint8_t type = 0; // ok (0), error(1)
			mavlink_msg_waypoint_ack_send(_channel, _cmdDestSysId,
					_cmdDestCompId, type);

			// turn off waypoint send
			_receivingCmds = false;
			break;
		}

		case MAVLINK_MSG_ID_WAYPOINT_CURRENT: {
			mavlink_msg_waypoint_current_send(_channel,
					_guide->getCurrentIndex());
			break;
		}

		default: {
			char msg[50];
			sprintf(msg, "autopilot sending unknown command with id: %d", id);
			sendText(SEVERITY_HIGH, msg);
		}

		} // switch
	} // send message

	virtual void receive() {
		//_hal->debug->printf_P(PSTR("receive\n"));
		// if number of channels exceeded return
		//
		if (_channel == MAVLINK_COMM_3)
			return;

		// receive new packets
		mavlink_message_t msg;
		mavlink_status_t status;

		// process received bytes
		while (comm_get_available(_channel)) {
			uint8_t c = comm_receive_ch(_channel);

			// Try to get a new message
			if (mavlink_parse_char(_channel, c, &msg, &status))
				_handleMessage(&msg);
		}

		// Update packet drops counter
		_packetDrops += status.packet_rx_drop_count;
	}

	void sendText(uint8_t severity, const char *str) {
		mavlink_msg_statustext_send(_channel, severity, (const int8_t*) str);
	}

	void sendText(uint8_t severity, const prog_char_t *str) {
		mavlink_statustext_t m;
		uint8_t i;
		for (i = 0; i < sizeof(m.text); i++) {
			m.text[i] = pgm_read_byte((const prog_char *) (str++));
		}
		if (i < sizeof(m.text))
			m.text[i] = 0;
		sendText(severity, (const char *) m.text);
	}

	void acknowledge(uint8_t id, uint8_t sum1, uint8_t sum2) {
	}

	/**
	 * sends parameters one at a time
	 */
	void sendParameters() {
		//_hal->debug->printf_P(PSTR("send parameters\n"));
		// Check to see if we are sending parameters
		while (NULL != _queuedParameter) {
			AP_Var *vp;
			float value;

			// copy the current parameter and prepare to move to the next
			vp = _queuedParameter;
			_queuedParameter = _queuedParameter->next();

			// if the parameter can be cast to float, report it here and break out of the loop
			value = vp->cast_to_float();
			if (!isnan(value)) {

				char paramName[_paramNameLengthMax];
				vp->copy_name(paramName, sizeof(paramName));

				mavlink_msg_param_value_send(_channel, (int8_t*) paramName,
						value, _countParameters(), _queuedParameterIndex);

				_queuedParameterIndex++;
				break;
			}
		}

	}

	/**
	 * request commands one at a time
	 */
	void requestCmds() {
		//_hal->debug->printf_P(PSTR("requesting commands\n"));
		// request cmds one by one
		if (_receivingCmds && _cmdRequestIndex <= _cmdNumberRequested) {
			mavlink_msg_waypoint_request_send(_channel, _cmdDestSysId,
					_cmdDestCompId, _cmdRequestIndex);
		}
	}

private:

	// options
	bool _useRelativeAlt;

	// commands
	bool _sendingCmds;
	bool _receivingCmds;
	uint16_t _cmdTimeLastSent;
	uint16_t _cmdTimeLastReceived;
	uint16_t _cmdDestSysId;
	uint16_t _cmdDestCompId;
	uint16_t _cmdRequestIndex;
	uint16_t _cmdNumberRequested;
	uint16_t _cmdMax;
	Vector<mavlink_command_t *> _cmdList;

	// parameters
	static uint8_t _paramNameLengthMax;
	uint16_t _parameterCount;
	AP_Var * _queuedParameter;
	uint16_t _queuedParameterIndex;

	// channel
	mavlink_channel_t _channel;
	uint16_t _packetDrops;
	static uint8_t _nChannels;

	void _handleMessage(mavlink_message_t * msg) {

		uint32_t timeStamp = micros();

		switch (msg->msgid) {
		_hal->debug->printf_P(PSTR("message received: %d"), msg->msgid);

		case MAVLINK_MSG_ID_HEARTBEAT: {
			mavlink_heartbeat_t packet;
			mavlink_msg_heartbeat_decode(msg, &packet);
			_hal->lastHeartBeat = micros();
			break;
		}

		case MAVLINK_MSG_ID_GPS_RAW: {
			// decode
			mavlink_gps_raw_t packet;
			mavlink_msg_gps_raw_decode(msg, &packet);

			_navigator->setTimeStamp(timeStamp);
			_navigator->setLat(packet.lat * deg2Rad);
			_navigator->setLon(packet.lon * deg2Rad);
			_navigator->setAlt(packet.alt);
			_navigator->setYaw(packet.hdg * deg2Rad);
			_navigator->setGroundSpeed(packet.v);
			_navigator->setAirSpeed(packet.v);
			//_hal->debug->printf_P(PSTR("received hil gps raw packet\n"));
			/*
			 _hal->debug->printf_P(PSTR("received lat: %f deg\tlon: %f deg\talt: %f m\n"),
			 packet.lat,
			 packet.lon,
			 packet.alt);
			 */
			break;
		}

		case MAVLINK_MSG_ID_ATTITUDE: {
			// decode
			mavlink_attitude_t packet;
			mavlink_msg_attitude_decode(msg, &packet);

			// set dcm hil sensor
			_navigator->setTimeStamp(timeStamp);
			_navigator->setRoll(packet.roll);
			_navigator->setPitch(packet.pitch);
			_navigator->setYaw(packet.yaw);
			_navigator->setRollRate(packet.rollspeed);
			_navigator->setPitchRate(packet.pitchspeed);
			_navigator->setYawRate(packet.yawspeed);
			//_hal->debug->printf_P(PSTR("received hil attitude packet\n"));
			break;
		}

		case MAVLINK_MSG_ID_ACTION: {
			// decode
			mavlink_action_t packet;
			mavlink_msg_action_decode(msg, &packet);
			if (_checkTarget(packet.target, packet.target_component))
				break;

			// do action
			sendText(SEVERITY_LOW, PSTR("action received"));
			switch (packet.action) {

			case MAV_ACTION_STORAGE_READ:
				AP_Var::load_all();
				break;

			case MAV_ACTION_STORAGE_WRITE:
				AP_Var::save_all();
				break;

			case MAV_ACTION_CALIBRATE_RC:
			case MAV_ACTION_CALIBRATE_GYRO:
			case MAV_ACTION_CALIBRATE_MAG:
			case MAV_ACTION_CALIBRATE_ACC:
			case MAV_ACTION_CALIBRATE_PRESSURE:
			case MAV_ACTION_REBOOT:
			case MAV_ACTION_REC_START:
			case MAV_ACTION_REC_PAUSE:
			case MAV_ACTION_REC_STOP:
			case MAV_ACTION_TAKEOFF:
			case MAV_ACTION_LAND:
			case MAV_ACTION_NAVIGATE:
			case MAV_ACTION_LOITER:
			case MAV_ACTION_MOTORS_START:
			case MAV_ACTION_CONFIRM_KILL:
			case MAV_ACTION_EMCY_KILL:
			case MAV_ACTION_MOTORS_STOP:
			case MAV_ACTION_SHUTDOWN:
			case MAV_ACTION_CONTINUE:
			case MAV_ACTION_SET_MANUAL:
			case MAV_ACTION_SET_AUTO:
			case MAV_ACTION_LAUNCH:
			case MAV_ACTION_RETURN:
			case MAV_ACTION_EMCY_LAND:
			case MAV_ACTION_HALT:
				sendText(SEVERITY_LOW, PSTR("action not implemented"));
				break;
			default:
				sendText(SEVERITY_LOW, PSTR("unknown action"));
				break;
			}
			break;
		}

		case MAVLINK_MSG_ID_WAYPOINT_REQUEST_LIST: {
			sendText(SEVERITY_LOW, PSTR("waypoint request list"));

			// decode
			mavlink_waypoint_request_list_t packet;
			mavlink_msg_waypoint_request_list_decode(msg, &packet);
			if (_checkTarget(packet.target_system, packet.target_component))
				break;

			// Start sending waypoints
			mavlink_msg_waypoint_count_send(_channel, msg->sysid, msg->compid,
					_guide->getNumberOfCommands());

			_cmdTimeLastSent = millis();
			_cmdTimeLastReceived = millis();
			_sendingCmds = true;
			_receivingCmds = false;
			_cmdDestSysId = msg->sysid;
			_cmdDestCompId = msg->compid;
			break;
		}

		case MAVLINK_MSG_ID_WAYPOINT_REQUEST: {
			sendText(SEVERITY_LOW, PSTR("waypoint request"));

			// Check if sending waypiont
			if (!_sendingCmds)
				break;

			// decode
			mavlink_waypoint_request_t packet;
			mavlink_msg_waypoint_request_decode(msg, &packet);
			if (_checkTarget(packet.target_system, packet.target_component))
				break;

			_hal->debug->printf_P(PSTR("sequence: %d\n"),packet.seq);
			AP_MavlinkCommand cmd(packet.seq);

			mavlink_waypoint_t wp = cmd.convert(_guide->getCurrentIndex());
			mavlink_msg_waypoint_send(_channel, _cmdDestSysId, _cmdDestCompId,
					wp.seq, wp.frame, wp.command, wp.current, wp.autocontinue,
					wp.param1, wp.param2, wp.param3, wp.param4, wp.x, wp.y,
					wp.z);

			// update last waypoint comm stamp
			_cmdTimeLastSent = millis();
			break;
		}

		case MAVLINK_MSG_ID_WAYPOINT_ACK: {
			sendText(SEVERITY_LOW, PSTR("waypoint ack"));

			// decode
			mavlink_waypoint_ack_t packet;
			mavlink_msg_waypoint_ack_decode(msg, &packet);
			if (_checkTarget(packet.target_system, packet.target_component))
				break;

			// check for error
			//uint8_t type = packet.type; // ok (0), error(1)

			// turn off waypoint send
			_sendingCmds = false;
			break;
		}

		case MAVLINK_MSG_ID_PARAM_REQUEST_LIST: {
			sendText(SEVERITY_LOW, PSTR("param request list"));

			// decode
			mavlink_param_request_list_t packet;
			mavlink_msg_param_request_list_decode(msg, &packet);
			if (_checkTarget(packet.target_system, packet.target_component))
				break;

			// Start sending parameters - next call to ::update will kick the first one out

			_queuedParameter = AP_Var::first();
			_queuedParameterIndex = 0;
			break;
		}

		case MAVLINK_MSG_ID_WAYPOINT_CLEAR_ALL: {
			sendText(SEVERITY_LOW, PSTR("waypoint clear all"));

			// decode
			mavlink_waypoint_clear_all_t packet;
			mavlink_msg_waypoint_clear_all_decode(msg, &packet);
			if (_checkTarget(packet.target_system, packet.target_component))
				break;

			// clear all waypoints
			uint8_t type = 0; // ok (0), error(1)
			_guide->setNumberOfCommands(1);
			_guide->setCurrentIndex(0);

			// send acknowledgement 3 times to makes sure it is received
			for (int i = 0; i < 3; i++)
				mavlink_msg_waypoint_ack_send(_channel, msg->sysid,
						msg->compid, type);

			break;
		}

		case MAVLINK_MSG_ID_WAYPOINT_SET_CURRENT: {
			sendText(SEVERITY_LOW, PSTR("waypoint set current"));

			// decode
			mavlink_waypoint_set_current_t packet;
			mavlink_msg_waypoint_set_current_decode(msg, &packet);
			Serial.print("Packet Sequence:");
			Serial.println(packet.seq);
			if (_checkTarget(packet.target_system, packet.target_component))
				break;

			// set current waypoint
			Serial.print("Current Index:");
			Serial.println(_guide->getCurrentIndex());
			Serial.flush();
			_guide->setCurrentIndex(packet.seq);
			mavlink_msg_waypoint_current_send(_channel,
					_guide->getCurrentIndex());
			break;
		}

		case MAVLINK_MSG_ID_WAYPOINT_COUNT: {
			sendText(SEVERITY_LOW, PSTR("waypoint count"));

			// decode
			mavlink_waypoint_count_t packet;
			mavlink_msg_waypoint_count_decode(msg, &packet);
			if (_checkTarget(packet.target_system, packet.target_component))
				break;

			// start waypoint receiving
			if (packet.count > _cmdMax) {
				packet.count = _cmdMax;
			}
			_cmdNumberRequested = packet.count;
			_cmdTimeLastReceived = millis();
			_receivingCmds = true;
			_sendingCmds = false;
			_cmdRequestIndex = 0;
			break;
		}

		case MAVLINK_MSG_ID_WAYPOINT: {
			sendText(SEVERITY_LOW, PSTR("waypoint"));

			// Check if receiving waypiont
			if (!_receivingCmds) {
				//sendText(SEVERITY_HIGH, PSTR("not receiving commands"));
				break;
			}

			// decode
			mavlink_waypoint_t packet;
			mavlink_msg_waypoint_decode(msg, &packet);
			if (_checkTarget(packet.target_system, packet.target_component))
				break;

			// check if this is the requested waypoint
			if (packet.seq != _cmdRequestIndex) {
				char warningMsg[50];
				sprintf(warningMsg,
				"waypoint request out of sequence: (packet) %d / %d (ap)",
				packet.seq, _cmdRequestIndex);
				sendText(SEVERITY_HIGH, warningMsg);
				break;
			}

			_hal->debug->printf_P(PSTR("received waypoint x: %f\ty: %f\tz: %f\n"),
					packet.x,
					packet.y,
					packet.z);

			// store waypoint
			AP_MavlinkCommand command(packet);
			//sendText(SEVERITY_HIGH, PSTR("waypoint stored"));
			_cmdRequestIndex++;
			if (_cmdRequestIndex == _cmdNumberRequested) {
				sendMessage(MAVLINK_MSG_ID_WAYPOINT_ACK);
				_receivingCmds = false;
				_guide->setNumberOfCommands(_cmdNumberRequested);
				//sendText(SEVERITY_LOW, PSTR("waypoint ack sent"));
			} else if (_cmdRequestIndex > _cmdNumberRequested) {
				_receivingCmds = false;
			}
			_cmdTimeLastReceived = millis();
			break;
		}

		case MAVLINK_MSG_ID_PARAM_SET: {
			sendText(SEVERITY_LOW, PSTR("param set"));
			AP_Var *vp;
			AP_Meta_class::Type_id var_type;

			// decode
			mavlink_param_set_t packet;
			mavlink_msg_param_set_decode(msg, &packet);
			if (_checkTarget(packet.target_system, packet.target_component))
				break;

			// set parameter

			char key[_paramNameLengthMax + 1];
			strncpy(key, (char *) packet.param_id, _paramNameLengthMax);
			key[_paramNameLengthMax] = 0;

			// find the requested parameter
			vp = AP_Var::find(key);
			if ((NULL != vp) && // exists
					!isnan(packet.param_value) && // not nan
					!isinf(packet.param_value)) { // not inf

				// add a small amount before casting parameter values
				// from float to integer to avoid truncating to the
				// next lower integer value.
				const float rounding_addition = 0.01;

				// fetch the variable type ID
				var_type = vp->meta_type_id();

				// handle variables with standard type IDs
				if (var_type == AP_Var::k_typeid_float) {
					((AP_Float *) vp)->set_and_save(packet.param_value);

				} else if (var_type == AP_Var::k_typeid_float16) {
					((AP_Float16 *) vp)->set_and_save(packet.param_value);

				} else if (var_type == AP_Var::k_typeid_int32) {
					((AP_Int32 *) vp)->set_and_save(
							packet.param_value + rounding_addition);

				} else if (var_type == AP_Var::k_typeid_int16) {
					((AP_Int16 *) vp)->set_and_save(
							packet.param_value + rounding_addition);

				} else if (var_type == AP_Var::k_typeid_int8) {
					((AP_Int8 *) vp)->set_and_save(
							packet.param_value + rounding_addition);
				} else {
					// we don't support mavlink set on this parameter
					break;
				}

				// Report back the new value if we accepted the change
				// we send the value we actually set, which could be
				// different from the value sent, in case someone sent
				// a fractional value to an integer type
				mavlink_msg_param_value_send(_channel, (int8_t *) key,
						vp->cast_to_float(), _countParameters(), -1); // XXX we don't actually know what its index is...
			}

			break;
		} // end case


		}
	}

	uint16_t _countParameters() {
		// if we haven't cached the parameter count yet...
		if (0 == _parameterCount) {
			AP_Var *vp;

			vp = AP_Var::first();
			do {
				// if a parameter responds to cast_to_float then we are going to be able to report it
				if (!isnan(vp->cast_to_float())) {
					_parameterCount++;
				}
			} while (NULL != (vp = vp->next()));
		}
		return _parameterCount;
	}

	AP_Var * _findParameter(uint16_t index) {
		AP_Var *vp;

		vp = AP_Var::first();
		while (NULL != vp) {

			// if the parameter is reportable
			if (!(isnan(vp->cast_to_float()))) {
				// if we have counted down to the index we want
				if (0 == index) {
					// return the parameter
					return vp;
				}
				// count off this parameter, as it is reportable but not
				// the one we want
				index--;
			}
			// and move to the next parameter
			vp = vp->next();
		}
		return NULL;
	}

	// check the target
	uint8_t _checkTarget(uint8_t sysid, uint8_t compid) {
		/*
		 char msg[50];
		 sprintf(msg, "target = %d / %d\tcomp = %d / %d", sysid,
		 mavlink_system.sysid, compid, mavlink_system.compid);
		 sendText(SEVERITY_LOW, msg);
		 */
		if (sysid != mavlink_system.sysid) {
			//sendText(SEVERITY_LOW, PSTR("system id mismatch"));
			return 1;

		} else if (compid != mavlink_system.compid) {
			//sendText(SEVERITY_LOW, PSTR("component id mismatch"));
			return 0; // XXX currently not receiving correct compid from gcs

		} else {
			return 0; // no error
		}
	}

};

} // namespace apo

#endif // AP_CommLink_H
// vim:ts=4:sw=4:tw=78:expandtab
