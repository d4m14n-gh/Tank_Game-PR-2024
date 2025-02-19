#include <sys/socket.h>
#include <netinet/in.h>
#include <arpa/inet.h>
#include <stdio.h>
#include <stdlib.h>
#include <unistd.h>
#include <string.h>
#include <pthread.h>

int connected[100];
char playersData[100][2000];
void *connection_handler(void *socket_desc) {
	
	/* Get the socket descriptor */
	int sock = * (int *)socket_desc;
	int read_size;
	char message[2000];
	 
	/* Send some messages to the client */
	sprintf(message, "%d\n\0", sock);
	write(sock , message , strlen(message));
	
	do {
		write(sock, "[", 1);
		for(int i=0;i<100;i++)
			if(connected[i] && sock!=i){
				write(sock, playersData[i], strlen(playersData[i]));
				write(sock, ", ", 2);
			}
		write(sock, "]\n", 2);

		read_size = recv(sock , playersData[sock] , 2000 , 0);
		if(recv<0){
			fprintf(stderr, "error receiving\n"); 
			break;
		}
		else if(recv==0){
			fprintf(stderr, "error client\n"); 
			break;
		}
		playersData[sock][read_size] = '\0';
		connected[sock] = 1;
	} while(read_size > 2); /* Wait for empty line */
	
	fprintf(stderr, "Client disconnected %d\n", sock); 
	
	connected[sock] = 0;
	close(sock);
	pthread_exit(NULL);
}

int main(int argc, char *argv[]) {
	int listenfd = 0, connfd = 0;
	struct sockaddr_in serv_addr; 
	
	pthread_t thread_id;

	listenfd = socket(AF_INET, SOCK_STREAM, 0);
	if(listenfd<0){
		fprintf(stderr, "Listen error\n");
		return 0;
	}
	memset(&serv_addr, '0', sizeof(serv_addr));

	serv_addr.sin_family = AF_INET;
	serv_addr.sin_addr.s_addr = htonl(INADDR_ANY);
	
	int port = 50080;
	if(argc>1)
		port = atoi(argv[1]);
	fprintf(stderr, "port: %d\n", port);
	
	
	serv_addr.sin_port = htons(port); 

	bind(listenfd, (struct sockaddr*)&serv_addr, sizeof(serv_addr)); 

	listen(listenfd, 32); 

	fprintf(stderr, "Server starts listening!\n"); 

	for (;;) {
		connfd = accept(listenfd, (struct sockaddr*)NULL, NULL);
		fprintf(stderr, "Connection accepted %d\n", connfd); 
		if(connfd<0){
			fprintf(stderr, "Connect accept error->break\n");
			break;
		}
		pthread_create(&thread_id, NULL, connection_handler , (void *) &connfd);
	}
}
