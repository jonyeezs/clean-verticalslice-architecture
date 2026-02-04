package demo.cleanslice.usecases.create_recipe;

import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

import an.awesome.pipelinr.Pipeline;
import java.util.concurrent.CompletableFuture;


@RestController
@RequestMapping("/recipes")
public class PostController {

    private final Pipeline mediator;

    public PostController(Pipeline mediator) {
        this.mediator = mediator;
    }

    @PostMapping("")
    public CompletableFuture<ResponseEntity<Response>> create(@RequestBody APIRequest request) {
        return mediator.send(request).thenApply(response -> {
            if (response.getId().isEmpty()) {
                return ResponseEntity.unprocessableEntity().build();
            }
            
            return ResponseEntity.status(HttpStatus.CREATED).body(response);
        });
    }

}
