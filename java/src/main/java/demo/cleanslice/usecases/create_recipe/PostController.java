package demo.cleanslice.usecases.create_recipe;

import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

@RestController
@RequestMapping("/recipes")
public class PostController {

    private Handler mediator;

    public PostController(Handler handler) {
        this.mediator = handler;
    }

    @PostMapping("")
    public ResponseEntity<Response> create(@RequestBody Request request) throws Exception {
        try {
            var response = mediator.handle(request);
            return ResponseEntity.status(HttpStatus.CREATED).body(response);

        } catch (RecipeExistsException e) {
            return ResponseEntity.unprocessableEntity().build();
        }
    }

}
