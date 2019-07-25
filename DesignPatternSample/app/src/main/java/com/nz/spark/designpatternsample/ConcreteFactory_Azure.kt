package com.nz.spark.designpatternsample

class ConcreteFactory_Azure : AbstractFactory_MessageQueue {
    override fun createProductA(): OutboundQueue {
        return AzureMessageQueue()
    }

    override fun createProductB(): ReplyQueue {
        return AzureResponseMessageQueue()
    }
}
