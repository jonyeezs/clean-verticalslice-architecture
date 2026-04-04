package demo.cleanslice.usecases.create_recipe;

import java.util.concurrent.CompletableFuture;

import an.awesome.pipelinr.Command;
import lombok.Getter;

/**
 * Represents a request to create a recipe.
 * 
 * @implNote When the API request schema starts to deviate from the internal dto,
 *           best to decouple them and have two different classes.
 */
public class APIRequest implements Command<CompletableFuture<Response>> {

    @Getter
    private String url;

    @Getter
    private String title;

    @Getter
    private String name;

    @Getter
    private String author;

    public APIRequest(String url, String title, String name, String author) {
        this.url = url;
        this.title = title;
        this.name = name;
        this.author = author;
    }
}
