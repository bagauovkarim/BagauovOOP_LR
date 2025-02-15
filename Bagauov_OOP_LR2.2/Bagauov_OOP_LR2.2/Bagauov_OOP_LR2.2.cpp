#include <iostream>
#include <string>

using namespace std;

class CPU {

private:
    string model;
    int core;
public:
    CPU() : core(0), model("") {
        printf("Вызван конструктор по умолчанию класса CPU \n");
    }
    CPU(int core, string model) : core(core), model(model) {
        printf("Вызван конструктор с параметрами класса CPU \n");
    }
    CPU(const CPU& cpu_copy) : core(cpu_copy.core), model(cpu_copy.model) {
        printf("Вызван конструктор копирования класса CPU \n");
    }
    ~CPU() {
        printf("Вызван деструктор класса CPU \n");
    }

    void core_setter(int c) {
        if (core > 0 && core < 512) {
            core = c;
        }
    }

    void model_setter(string m) {
        model = m;
    }

    int core_getter() {
        return core;
    }

    string model_getter() {
        return model;
    }

    virtual void print_info() {

        printf("Model info: %d\n", core);

        printf("Core info: %s\n", model.c_str());
    }

};

class Server {
private:
    CPU* cpu1;
    CPU* cpu2;
public:
    Server() : cpu1(new CPU()), cpu2(new CPU()) {
        printf("Вызван конструктор по умолчанию класса Server \n");
    }
    Server(int c1, int c2, string m1, string m2) : cpu1(new CPU(c1, m1)), cpu2(new CPU(c2, m2)) {
        printf("Вызван конструктор с параметрами класса Server \n");
    }
    Server(const Server& server_copy) : cpu1(new CPU(*server_copy.cpu1)), cpu2(new CPU(*server_copy.cpu2)) {
        printf("Вызван конструктор копирования класса Server \n");
    }
    ~Server() {
        delete cpu1;
        delete cpu2;
        printf("Вызван деструктор класса Server \n");
    }

    void server_setter(int c1, string m1, int c2, string m2) {
        cpu1->core_setter(c1);
        cpu1->model_setter(m1);
        cpu2->core_setter(c2);
        cpu2->model_setter(m2);

    }

    /*/CPU* server_getter() {
        return cpu1;
    }*/
    
    void print_info() {
        printf("Server info: \n");
        cpu1->print_info();
        cpu2->print_info();


    }
};



int main() {
    setlocale(LC_ALL, "RU");

    Server server1;
    Server server2(1, 4, "Intel1", "Intel2");
    Server server3 = server2;

    Server* server4 = new Server();
    Server* server5 = new Server(5, 6, "Intel3", "Intel4");
    Server* server6 = new Server(*server5);

    server2.print_info();

    delete server4;
    delete server5;
    delete server6;


}