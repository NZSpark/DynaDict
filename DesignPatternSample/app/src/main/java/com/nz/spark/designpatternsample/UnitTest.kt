package com.nz.spark.designpatternsample

//2 cases tested.
class UnitTest {

    fun TestCase() {
        val cfa = ConcreteFactory_Azure()
        val cfm = ConcreteFactory_Msmq()

        //dependency injection
        val C1 = Client(cfa)
        val C2 = Client(cfm)

        //ConcreteFactory_Azure test case
        C1.sendMessage()
        println(C1.receiveMessage())

        //ConcreteFactory_Msmq test case
        C2.sendMessage()
        println(C2.receiveMessage())
    }
}
