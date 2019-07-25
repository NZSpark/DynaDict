package com.nz.spark.designpatternsample

interface AbstractFactory_MessageQueue {
    fun createProductA(): OutboundQueue
    fun createProductB(): ReplyQueue
}
