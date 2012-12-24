#include <Arduino.h>
#include <Comm\Serial\ScoeComms.h>
#include <Servo\Servo.h>
#include <Encoder\Encoder.h>
#include <RobotModel\RobotModel.h>
#include <RobotModel\RslModelSection.h>
#include <Comm\Udp\UdpComms.h>
#include <Utils\utils.h>
#include <Control\PID.h>
#include <Arduino.h>
#include <Drivers\Output\RCMotor.h>
#include <Drivers\Input\ADXL345.h>
#include <Drivers\Input\ADXL345Axis.h>
#include <Drivers\Input\ITG3200.h>
#include <Drivers\Input\ITG3200Axis.h>
#include <Control\SimpleAngleThing.h>
#include "BalanceSection.h"
#include "BalBot.h"


#define BALANCE_SAFTEY_TILT 6

#define LEFT_MOTOR 3
#define RIGHT_MOTOR 2
#define RIGHT_INVERT 1
#define LEFT_INVERT -1


/*AnalogGyro TiltGyro(9, 1);
AnalogAccelerometer UpAccel(12, 500, true);
AnalogAccelerometer ForwardAccel(13, 500, true);
SimpleAngleThing AngleCalc(&TiltGyro, &ForwardAccel, &UpAccel, 0.98, true);
*/

Accelerometer *vertAccel;
Accelerometer *horizAccel;
Gyro *tiltGyro;

ITG3200 *gyro;
ADXL345 *accel;
SimpleAngleThing *angleCalc;

Encoder leftEnc (21, 18);
Encoder rightEnc (20,17);

Servo left;
Servo right;

#define PULSE_RANGE 750
#define LEFT_CENTER 1500
#define RIGHT_CENTER 1490
#define PLUSE_DEADBAND 80

//PID BalancePID(-40, -2, -25);
PID BalancePID(0, 0, 0);

//SerialInterface beagleComm;
UdpComms udpComm;
TuningDataSection tuningData;
RslModelSection rsl;

void setup() {
	
	pinMode(13, OUTPUT);
	writeLed(true);
	Serial.begin(115200);
	
	Serial.println("Starting up...");
	Serial.println("Starting ethernet...");
	
	udpComm.isDhcp = true;
	udpComm.receivePort = 8888;
	byte mac[6] = {
		0xAA, 0xBB, 0xCC, 0xDD, 0xEE, 0xFF
	};
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
	tuningData.p = -0.08f;
	tuningData.i = -0.005f;
	tuningData.d = 0.0005f;
	tuningData.desiredAngle = -8;
	tuningData.spin = 0;
	tuningData.safteyLimit = 3;


	Serial.println();

	gyro = new ITG3200();


	Serial.println("Waiting to calibrate...");
	delay(1000);
	Serial.println("Calibrating...");
	gyro->calibrate();
	Serial.println("Calibration complete.");


	accel = new ADXL345();


	vertAccel = accel->getY();
	vertAccel->setInvert(true);
	horizAccel = accel->getZ();
	horizAccel->setInvert(true);
	tiltGyro = gyro->getX();

	angleCalc = new SimpleAngleThing(tiltGyro, horizAccel, vertAccel, 0.98, false);

	right.attach(RIGHT_MOTOR);
	left.attach(LEFT_MOTOR);

	Serial.println("Ready.");
	writeLed(false);
}

unsigned long lastLoop = 0;
float elapsedSeconds = 0.0;
void loop() {
	normalLoop();
	//printDigitalImuCsv();
	//printAngle();
}
void normalLoop(){
	long nowMicros = micros();
	if (lastLoop == 0)
	lastLoop = nowMicros;
	elapsedSeconds = (float)(nowMicros - lastLoop) / 1000000;
	
	//beagleComm.poll();
	udpComm.poll();
	
	if (elapsedSeconds >= 0.01){
		gyro->update();
		accel->update();
		//Serial.println(gyro->elapsedSeconds, 3);
		BalancePID.P = tuningData.p;
		BalancePID.I = tuningData.i;
		BalancePID.D = tuningData.d;
		
		/*BalancePID.P = -0.08f;
		BalancePID.I = -0.005f;
		BalancePID.D = 0.0005f;
		*/
		
		lastLoop = nowMicros;
		
		balance(tuningData.desiredAngle, tuningData.spin);
		tuningData.currentAngle = angleCalc->angle;
		printAngle();
		//printImuCsv();
		//testCenter();
		//printAngleCalcCsv();
	}
}
void printAngleCalcCsv(){
	Serial.print(horizAccel->getAcceleration(), 3);
	Serial.print(",");
	Serial.print(vertAccel->getAcceleration(), 3);
	Serial.print(",");
	Serial.print(tiltGyro->getRate(), 3);
	Serial.print(",");
	Serial.print(gyro->elapsedSeconds, 3);
	Serial.print(",");
	Serial.print(tiltGyro->getDeltaAngle(), 3);
	Serial.print(",");
	Serial.print(micros());
	Serial.print(",");
	Serial.print(angleCalc->angle, 3);
	Serial.println();
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
int count = 0;
void printImuCsv(){
	int anas[6];
	for (int i = 0; i < 6; i++){
		anas[i] = analogRead(i+8);
	}
	
	Serial.print(count);
	Serial.print(",");
	for (int i = 0; i < 6; i++){
		Serial.print(anas[i]);
		Serial.print(",");
	}
	Serial.print(micros());
	
	Serial.println();
	//delay(1);
}

void writeLed(bool state){
	digitalWrite(13, state ? HIGH : LOW);
}

void balance(float desiredAngle, float spin){
	angleCalc->update();
	float output = 0.0;
	float error = desiredAngle - angleCalc->angle;
	
	if (abs(error) > tuningData.safteyLimit){
		output = 0;
		BalancePID.iTotal = 0.0;
		spin = 0;
	}
	else{
		output = BalancePID.update(angleCalc->angle, desiredAngle, angleCalc->gyro->getRate());
	}
	setDrive(output + spin, output - spin);
}

void setDrive(float leftVal, float rightVal){
	leftVal *= LEFT_INVERT;
	rightVal *= RIGHT_INVERT;
	leftVal = limit(leftVal, -1.0, 1.0);
	rightVal = limit(rightVal, -1.0, 1.0);
	
	leftVal *= PULSE_RANGE;
	rightVal *= PULSE_RANGE;
	
	left.writeMicroseconds(LEFT_CENTER + removeDeadband(leftVal, PLUSE_DEADBAND, PULSE_RANGE));
	right.writeMicroseconds(RIGHT_CENTER + removeDeadband(rightVal, PLUSE_DEADBAND, PULSE_RANGE));
}

/*void calibrateAnalog(int calibrationDelay, int calibrationLoops) {
	Serial.println("Waiting to calibrate...");
	delay(calibrationDelay);
	Serial.println("Calibrating...");

	for (int i = 0; i < calibrationLoops; i++) {
		TiltGyro.calibrate();
		delay(5);
	}
	
	TiltGyro.endCalibrate();

	Serial.println("Calibration complete!");
	Serial.print("GyroCenter:");
	Serial.print(TiltGyro.getCenterValue());
}*/