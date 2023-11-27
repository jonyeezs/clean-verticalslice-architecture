package demo.cleanslice.usecases.create_recipe;

import java.util.UUID;

import lombok.Data;

@Data
public class Response {
    private UUID id;

    public Response(UUID id) {
        this.id = id;
    }

}
