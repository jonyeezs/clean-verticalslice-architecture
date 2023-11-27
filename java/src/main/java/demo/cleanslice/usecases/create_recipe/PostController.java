package demo.cleanslice.usecases.create_recipe;

import org.springframework.web.bind.annotation.PostMapping;
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
    public Response create(Request request) {
        return mediator.handle(request);
    }

}
