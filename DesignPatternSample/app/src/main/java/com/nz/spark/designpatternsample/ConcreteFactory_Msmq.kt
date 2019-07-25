package com.nz.spark.designpatternsample

class ConcreteFactory_Msmq : AbstractFactory_MessageQueue {
    override fun createProductA(): OutboundQueue {
        return MsmqMessageQueue()
    }

    override fun createProductB(): ReplyQueue {
        return MsmqResponseMessageQueue()
    }
}