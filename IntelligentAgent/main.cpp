#include "stdafx.h"

using json = nlohmann::json;

int main(int argc, char* argv[])
{
	(void)argc;

	json firstJson;
	std::ifstream input(argv[1]);
	std::ofstream output(argv[2]);

	input >> firstJson;
	output << firstJson;

	return 0;
}