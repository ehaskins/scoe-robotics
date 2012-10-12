struct EncoderDefinition{
	public:
		bool Enabled;
		unsigned char ChannelA;
		unsigned char ChannelB;
		int TicksPerRotation;
		long Position; //in ticks
}

struct PwmDefinition{
	public:
		bool Enabled;
		unsigned char Pin;
		unsigned char value;
}

struct DigitalIODefinition{
	public:
		bool Enabled;
		unsigned char Pin;
		bool Output;
		bool Value;
}

struct AnalogInputDefinition{
	public:
		bool Enabled;
		unsigned char Pin;
		unsigned int Value;
}

void main(){

	while (1){

	}
}