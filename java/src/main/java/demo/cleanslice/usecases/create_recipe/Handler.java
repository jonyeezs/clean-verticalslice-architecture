package demo.cleanslice.usecases.create_recipe;

import java.util.UUID;

import org.springframework.stereotype.Service;

@Service
public class Handler {

    public Response handle(Request request) {
        return new Response(UUID.randomUUID());
    }
}
