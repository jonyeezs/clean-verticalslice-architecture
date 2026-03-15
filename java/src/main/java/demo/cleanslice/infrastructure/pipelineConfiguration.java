package demo.cleanslice.infrastructure;

import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;
import org.springframework.beans.factory.ObjectProvider;

import an.awesome.pipelinr.Pipeline;
import an.awesome.pipelinr.Pipelinr;
import an.awesome.pipelinr.Command;
import an.awesome.pipelinr.Notification;


@Configuration
class PipelineConfiguration {

    // Required as the library does not support streaming generic handlers
    @SuppressWarnings({ "rawtypes" })
    @Bean
    Pipeline pipeline(
        ObjectProvider<Command.Handler> commandHandlers, ObjectProvider<Notification.Handler> notificationHandlers, ObjectProvider<Command.Middleware> middlewares) {
        return new Pipelinr()
          .with(commandHandlers::stream)
          .with(notificationHandlers::stream)
          .with(middlewares::orderedStream);
    }
}