package demo.cleanslice.usecases.create_recipe;

import java.util.Optional;
import java.util.UUID;

import lombok.Data;

@Data
public class Response {
    private Optional<UUID> id;

    public Response(Optional<UUID> id) {
        this.id = id;
    }
}
