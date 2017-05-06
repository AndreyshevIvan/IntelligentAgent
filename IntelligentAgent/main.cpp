#include "stdafx.h"

namespace
{
	//const std::string MOOPED_HOST = "www.boost.org";
	//const std::string REQUEST = "/LICENSE_1_0.txt";
	const std::string MOOPED_HOST = "mooped.net";
	const std::string REQUEST = "/local/its/index.php?module=game&action=agentaction&gameid=32&userid=3568&act=noAct%20noAct";
	const std::string PORT = "443";
}

using boost::asio::ip::tcp;

int main()
{
	try
	{
		boost::asio::io_service io_service;

		tcp::resolver resolver(io_service);
		tcp::resolver::query query(MOOPED_HOST, PORT);
		tcp::resolver::iterator endpoint_iterator = resolver.resolve(query);
		tcp::resolver::iterator end;

		tcp::socket socket(io_service);
		boost::system::error_code error = boost::asio::error::host_not_found;
		while (error && endpoint_iterator != end)
		{
			socket.close();
			socket.connect(*endpoint_iterator++, error);
		}
		if (error)
		{
			throw boost::system::system_error(error);
		}

		boost::asio::streambuf request;
		std::ostream request_stream(&request);
		request_stream << "GET " << REQUEST << " HTTPS/1.1\r\n";
		request_stream << "Host: " << MOOPED_HOST << "\r\n";
		request_stream << "Accept: */*\r\n";
		request_stream << "Connection: close\r\n\r\n";

		boost::asio::write(socket, request);

		boost::asio::streambuf response;
		boost::asio::read_until(socket, response, "\r\n");

		std::istream response_stream(&response);
		std::string http_version;
		response_stream >> http_version;
		unsigned int status_code;
		response_stream >> status_code;
		std::string status_message;
		std::getline(response_stream, status_message);
		if (!response_stream || http_version.substr(0, 5) != "HTTP/")
		{
			std::cout << "Invalid response\n";
			return 1;
		}
		if (status_code != 200)
		{
			std::cout << "Response returned with status code " << status_code << "\n";
			std::string respLine;
			while (getline(response_stream, respLine))
			{
				std::cout << respLine << std::endl;
			}
			return 1;
		}

		boost::asio::read_until(socket, response, "\r\n\r\n");

		// Парсим заголовки
		std::string header;
		while (std::getline(response_stream, header) && header != "\r")
			std::cout << header << "\n";
		std::cout << "\n";

		// Выводим в лог
		if (response.size() > 0)
			std::cout << &response;

		// Теперь читаем до конца
		while (boost::asio::read(socket, response,
			boost::asio::transfer_at_least(1), error))
			std::cout << &response;

		if (error != boost::asio::error::eof) // ошибка
			throw boost::system::system_error(error);
	}
	catch (std::exception& e) // возникло исключение
	{
		std::cout << "Exception: " << e.what() << "\n";
	}

	return 0;
}