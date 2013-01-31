/*
* Skateboard.cpp
*
* Created: 12/17/2012 3:13:01 PM
*  Author: EHaskins
*/

#include <Arduino.h>
#include <Drivers\Output\Osmc.h>
#include <Drivers\Output\RCMotor.h>
#include <Drivers\Input\ADXL345.h>
#include <Drivers\Input\ADXL345Axis.h>
#include <Drivers\Input\ITG3200.h>
#include <Drivers\Input\ITG3200Axis.h>
#include <Control\SimpleAngleThing.h>
#include <Control\PID.h>
#include <Comm\Udp\UdpComms.h>

#include <Robotmodel\RslModelSection.h>

#include "BalanceSection.h"
#include "Skateboard.h"


#define SPIN_DEADBAND 0.05
#define SPIN_FACTOR 0.15;

//#define ENABLE_UDP
Osmc right(6, 5, 7);
Osmc left(9, 11, 8);

Accelerometer *vertAccel;
Accelerometer *horizAccel;
Gyro *tiltGyro;

ITG3200 *gyro;
ADXL345 *accel;
SimpleAngleThing *angleCalc;

unsigned long lastPrint;

float balancePoint = 3.5;
float p = 0.08;
float i = 0.003;
float d = -0.0007;
float safteyLimit = 15;
float startTolerance = 2;

PID balancePid(p, i, d);

#ifdef ENABLE_UDP
//SerialInterface beagleComm;
UdpComms udpComm;
TuningDataSection tuningData;
RslModelSection rsl;

byte mac[6] = {
	0xAA, 0xBB, 0xCC, 0xDD, 0xEE, 0xFF
};
IPAddress ip(192, 168, 10, 2);
IPAddress gw(0, 0, 0, 0);
IPAddress mask(255, 255, 255 ,0);
#endif

void setup(){
	Serial.begin(115200);
	
	while(1){
		Serial.println(analogRead(3));
	Serial.println(analogRead(4));
	Serial.println(analogRead(5));
	}	
	#ifdef ENABLE_UDP
	Serial.println("Starting up...");
	Serial.println("Starting ethernet...");
	
	Ethernet.begin(mac, ip, gw, mask);
	udpComm.isDhcp = false;
	udpComm.receivePort = 8888;
	for (int i = 0; i< 6; i++){
		udpComm.macAddress[i] = mac[i];
	}
	udpComm.init();
	// print your local IP address:
	Serial.print("My IP address: ");
	for (byte i = 0; i < 4; i++) {
		// print the value of each byte of the IP address:
		Serial.print(Ethernet.localIP()[i], DEC);
		Serial.print(".");
	}
	
	udpComm.robotModel.addSection(&tuningData);
	udpComm.robotModel.addSection(&rsl);
	
	#endif

	Serial.println("Waiting to calibrate...");
	gyro = new ITG3200();
	delay(1000);
	Serial.println("Calibrating...");
	gyro->calibrate();
	Serial.println("Calibration complete.");


	accel = new ADXL345();

	Serial.println("Accel initialized!");

	vertAccel = accel->getZ();
	vertAccel->setInvert(false);
	horizAccel = accel->getX();
	horizAccel->setInvert(false);
	tiltGyro = gyro->getY();
	tiltGyro->setInvert(true);

	angleCalc = new SimpleAngleThing(tiltGyro, horizAccel, vertAccel, 0.98, false);
	
	right.setIsEnabled(true);
	left.setIsEnabled(true);
	
	delay(200);
	
	#ifdef ENABLE_UDP
	tuningData.p = p;
	tuningData.i = i;
	tuningData.d = d;
	tuningData.safteyLimit = safteyLimit;
	#endif
	
	balancePid.P = p;
	balancePid.I = i;
	balancePid.D = d;
	
	pinMode(3, INPUT);
	digitalWrite(3, HIGH);
	pinMode(12, INPUT);
	digitalWrite(12, HIGH);

	pinMode(13, OUTPUT);
	digitalWrite(13, HIGH);
	Serial.println("ready!");
	lastPrint = 0;
}


int loopCount = 0;
void loop(){
	mainLoop();
	//printTurn();
	/*accel->update();
	gyro->update();
	angleCalc->update();
	printAngle();*/
	//printSafties();
	//testDrive();
	//printDigitalImuCsv();
	//delay(100);
}

void printSafties(){
	Serial.print("3:");
	Serial.print(digitalRead(3));
	Serial.print("  12:");
	Serial.println(digitalRead(12));
	Serial.println(checkSafties());
}
void printTurn(){
	Serial.print(analogRead(3));
	Serial.print("   ");
	Serial.print(getTurn());
	Serial.print("   ");
	Serial.println(deadBand(getTurn(), SPIN_DEADBAND, 1));
}

float getTurn(){
	float input = (float)analogRead(3) - 45;
	if (input < 0){
		input = 0;
	}
	return -(sqrt(input) - 14) / 14;
}

void mainLoop(){
	#ifdef ENABLE_UDP
	udpComm.poll();
	#endif
	
	if (accel->update())
	{
		gyro->update();
		angleCalc->update();
		if (loopCount++ % 10 == 0){
			printAngle();
		}
		
		#ifdef ENABLE_UDP
		balancePid.P = tuningData.p;
		balancePid.I = tuningData.i;
		balancePid.D = tuningData.d;
		safteyLimit = tuningData.safteyLimit;
		tuningData.currentAngle = angleCalc->angle;

		balance(tuningData.desiredAngle, tuningData.spin);

		#else
		balance(balancePoint, getTurn());
		#endif
		/*BalancePID.P = -0.08f;
		BalancePID.I = -0.005f;
		BalancePID.D = 0.0005f;
		*/
		
	}
}

bool lastLoopPowerEnabled = false;
void balance(float desiredAngle, float spin){
	float balancePower = 0.0, lSpin = 0, rSpin = 0;;
	float error = desiredAngle - angleCalc->angle;
	
	
	float limit = lastLoopPowerEnabled ? safteyLimit : startTolerance;
	bool riderPresent = checkSafties();
	bool powerEnabled = riderPresent && abs(error) < limit;
	
	if (powerEnabled){
		balancePower = balancePid.update(angleCalc->angle, desiredAngle, angleCalc->gyro->getRate());
		
		spin = deadBand(spin, SPIN_DEADBAND, 1);

		float absBalancePower = balancePower > 0 ? balancePower : -balancePower;

		float spinFactor = SPIN_FACTOR;
		spinFactor *= 1-absBalancePower;

		spin *= spinFactor;
		
		#ifdef ENABLE_UDP
		spin = tuningData.spin;
		#endif
		
		if (spin > 0){
			//lSpin = spin + spin * absBalancePower;
			//rSpin = spin * 1-absBalancePower;

			lSpin = 2 * spin;
		}
		else if (spin < 0){
			//lSpin = spin * 1-absBalancePower;
			//rSpin = spin + spin * absBalancePower;
			
			rSpin = 2 * spin;
		}
	}
	else{
		balancePower = 0;
		balancePid.iTotal = 0.0;
		rSpin = lSpin = 0;
	}

	setDrive(balancePower + lSpin, balancePower - rSpin);
	lastLoopPowerEnabled = powerEnabled;
}

bool checkSafties(){
	return digitalRead(3) == LOW && digitalRead(12) == LOW;
}
void setDrive(float leftPow, float rightPow){
	left.setOutput(leftPow);
	right.setOutput(-rightPow);
}

void printAngle(){
	Serial.print("Angle:");
	Serial.print(angleCalc->angle);
	Serial.print(" Accel X:");
	Serial.print(horizAccel->getAcceleration());
	Serial.print(" Accel Y:");
	Serial.print(vertAccel->getAcceleration());
	Serial.print(" Rate:");
	Serial.println(tiltGyro->getRate());
}
void printSensors(){
	//if (accel->update()){
		Serial.print(accel->getX()->getAcceleration());
		Serial.print(",");
		Serial.print(accel->getY()->getAcceleration());
		Serial.print(",");
		Serial.print(accel->getZ()->getAcceleration());
	//}
	//if (gyro->update()){
		unsigned long now = micros();
		Serial.print(",");
		Serial.print(gyro->getX()->getRate());
		Serial.print(",");
		Serial.print(gyro->getY()->getRate());
		Serial.print(",");
		Serial.print(gyro->getZ()->getRate());
		Serial.print(",");
		Serial.println(now-lastPrint);
		lastPrint = now;
	//}
}

void printDigitalImuCsv(){
	if (accel->update())
	{
		gyro->update();
		
		Serial.print(accel->getX()->getRawValue());
		Serial.print(",");
		Serial.print(accel->getY()->getRawValue());
		Serial.print(",");
		Serial.print(accel->getZ()->getRawValue());
		Serial.print(",");
		
		Serial.print(gyro->getX()->getRawValue());
		Serial.print(",");
		Serial.print(gyro->getY()->getRawValue());
		Serial.print(",");
		Serial.print(gyro->getZ()->getRawValue());
		Serial.print(",");
		Serial.println(micros());
	}
}

float drive = 0;
float max = 0.15;
float min = -0.15;
float dir = 0.01;
void testDrive(){
	drive += dir;
	
	if (drive < min || drive > max)
	dir *= -1;
	
	right.setOutput(drive);
	left.setOutput(drive);
	Serial.println(drive);
	delay(100);
}
float deadBand(float val, float deadband, float range){
	float center = 0;
	float scale = (range - deadband)/range;
	
	float out;
	if (val > center - deadband && val < center + deadband){
		return 0;
	}
	
	if (val < center){
		deadband *= -1;
	}
	
	out = (val - deadband) * scale;
	return out;
}